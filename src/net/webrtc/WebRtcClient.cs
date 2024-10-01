using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace NightmareNegotiations.net.webrtc;

public partial class WebRtcClient : Node
{
	[Export]
	public bool ShouldMesh { get; set; }
	private WebSocketPeer ws;

	public override void _Ready()
	{
		ws = new WebSocketPeer();

		var error = Connect("ws://127.0.0.1:45567");
		if (error == Error.Ok)
		{
			GD.Print("[Log] Successfully connected to signaling server!");
		}
		else
		{
			GD.Print($"[Error] Could not connect to signaling server: {error}");
		}
	}

	public Error Connect(string url)
	{
		Close();
		return ws.ConnectToUrl(url);
	}

	public void Close()
	{
		ws.Close();
	}

	public override void _Process(double delta)
	{
		ws.Poll();
		var wsReadyState = ws.GetReadyState();
		while (wsReadyState == WebSocketPeer.State.Open && ws.GetAvailablePacketCount() > 0)
		{
			if (!ParseMessage())
			{
				GD.PrintErr("[Error] Could not parse message from server.");
			}
		}
	}

	private bool ParseMessage()
	{
		var packet = JsonSerializer.Deserialize<Message>(ws.GetPacket().GetStringFromUtf8());
		if (packet == null) return false;

		var type = packet.Type;
		var srcPeerId = packet.Id;
		var data = packet.Data;

		switch (type)
		{
			case MessageType.Id:
				EmitSignal(SignalName.Connected, srcPeerId, data == "true");
				break;
			case MessageType.JoinLobby:
				EmitSignal(SignalName.LobbyJoined, data);
				break;
			case MessageType.PeerConnect:
				EmitSignal(SignalName.PeerConnected, srcPeerId);
				break;
			case MessageType.PeerDisconnect:
				EmitSignal(SignalName.PeerDisconnected, srcPeerId);
				break;
			case MessageType.Offer:
				EmitSignal(SignalName.OfferReceived, data);
				break;
			case MessageType.Answer:
				EmitSignal(SignalName.AnswerReceived, data);
				break;
			case MessageType.Candidate:
				var candidate = data.Split('\n');
				
				if (candidate.Length != 3) return false;
				if (!candidate[1].IsValidInt()) return false;
				
				EmitSignal(SignalName.CandidateReceived, candidate[0], candidate[1].ToInt(), candidate[2]);
				break;
			case MessageType.Seal:
				EmitSignal(SignalName.LobbySealed);
				break;
			default:
				return false;
		}
		
		return true;
	}

	public Error JoinLobby(string lobbyCode)
	{
		return SendMessage(MessageType.JoinLobby, ShouldMesh ? 0 : 1, lobbyCode);
	}

	public Error SealLobby()
	{
		return SendMessage(MessageType.Seal, 0);
	}

	public Error SendCandidate(int id, string media, int index, string sdp)
	{
		return SendMessage(MessageType.Candidate, id, $"{media}\n{index}\n{sdp}");
	}

	public Error SendOffer(int id, string offer)
	{
		return SendMessage(MessageType.Offer, id, offer);
	}
	
	public Error SendAnswer(int id, string answer)
	{
		return SendMessage(MessageType.Answer, id, answer);
	}
	
	private Error SendMessage(MessageType type, int id, string data = "")
	{
		return ws.SendText(JsonSerializer.Serialize(new Message
		{
			Type = type,
			Id = id,
			Data = data,
		}));
	}
	
	[Signal]
	public delegate void LobbyJoinedEventHandler(string lobbyId);
	[Signal]
	public delegate void ConnectedEventHandler(int id, bool useMesh);
	[Signal]
	public delegate void DisconnectedEventHandler();
	[Signal]
	public delegate void PeerConnectedEventHandler(int id);
	[Signal]
	public delegate void PeerDisconnectedEventHandler(int id);
	[Signal]
	public delegate void OfferReceivedEventHandler(int id, string offer);
	[Signal]
	public delegate void AnswerReceivedEventHandler(int id, string answer);
	[Signal]
	public delegate void CandidateReceivedEventHandler(int id, string media, int index, string sdp);
	[Signal]
	public delegate void LobbySealedEventHandler();
}