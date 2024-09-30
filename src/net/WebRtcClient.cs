using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace NightmareNegotiations.net;

public partial class WebRtcClient : Node
{
	[Export]
	public bool ShouldMesh { get; set; }
	
	private WebSocketPeer ws;

	public override void _Ready()
	{
		ws = new WebSocketPeer();
	}

	public void Connect(string url)
	{
		Close();
		ws.ConnectToUrl(url);
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
		var packet = JsonSerializer.Deserialize<Packet>(ws.GetPacket().GetStringFromUtf8());
		if (packet == null) return false;

		var type = packet.Type;
		var srcPeerId = packet.Id;
		var data = packet.Data;

		switch (type)
		{
			case Message.Id:
				EmitSignal(SignalName.Connected, srcPeerId, data == "true");
				break;
			case Message.JoinLobby:
				EmitSignal(SignalName.LobbyJoined, data);
				break;
			case Message.PeerConnect:
				EmitSignal(SignalName.PeerConnected, srcPeerId);
				break;
			case Message.PeerDisconnect:
				EmitSignal(SignalName.PeerDisconnected, srcPeerId);
				break;
			case Message.Offer:
				EmitSignal(SignalName.OfferReceived, data);
				break;
			case Message.Answer:
				EmitSignal(SignalName.AnswerReceived, data);
				break;
			case Message.Candidate:
				var candidate = data.Split('\n');
				
				if (candidate.Length != 3) return false;
				if (!candidate[1].IsValidInt()) return false;
				
				EmitSignal(SignalName.CandidateReceived, candidate[0], candidate[1].ToInt(), candidate[2]);
				break;
			case Message.Seal:
				EmitSignal(SignalName.LobbySealed);
				break;
			default:
				return false;
		}
		
		return true;
	}

	public Error JoinLobby(string lobbyCode)
	{
		return SendMessage(Message.JoinLobby, ShouldMesh ? 0 : 1, lobbyCode);
	}

	public Error SealLobby()
	{
		return SendMessage(Message.Seal, 0);
	}

	public Error SendCandidate(int id, string media, int index, string sdp)
	{
		return SendMessage(Message.Candidate, id, $"{media}\n{index}\n{sdp}");
	}

	public Error SendOffer(int id, string offer)
	{
		return SendMessage(Message.Offer, id, offer);
	}
	
	public Error SendAnswer(int id, string answer)
	{
		return SendMessage(Message.Answer, id, answer);
	}
	
	private Error SendMessage(Message type, int id, string data = "")
	{
		return ws.SendText(JsonSerializer.Serialize(new Packet
		{
			Type = type,
			Id = id,
			Data = data,
		}));
	}

	private class Packet
	{
		[JsonPropertyName("type")] public Message Type { get; set; }
		[JsonPropertyName("id")] public long Id { get; set; }
		[JsonPropertyName("data")] public string Data { get; set; }
	}

	public enum Message
	{
		Id,
		JoinLobby,
		PeerConnect,
		PeerDisconnect,
		Offer,
		Answer,
		Candidate,
		Seal
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