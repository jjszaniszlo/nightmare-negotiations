using System.Text.Json;
using Godot;

namespace NightmareNegotiations.net;

public partial class Client : Node
{
	private WebRtcMultiplayerPeer rtcMultiplayerPeer = new();
	private WebSocketPeer wsPeer = new();
	private string url = "ws://127.0.0.1:4556";
	private bool connected = false;

	[Signal]
	public delegate void InvalidNewLobbyNameEventHandler();
	
	[Signal]
	public delegate void InvalidJoinLobbyNameEventHandler();
	
	[Signal]
	public delegate void NewLobbyEventHandler(string lobbyName);
	
	[Signal]
	public delegate void JoinLobbyEventHandler(string lobbyName);
	
	[Signal]
	public delegate void LobbyListReceivedEventHandler(string[] lobbyList);
	
	[Signal]
	public delegate void LobbyMessageReceivedEventHandler(string userName, string message);

	[Signal]
	public delegate void AnotherUserJoinedLobbyEventHandler(string anotherUserName);
	
	[Signal]
	public delegate void HostNameReceivedEventHandler(string hostName);
	
	[Signal]
	public delegate void AnotherUserLeftLobbyEventHandler(string anotherUserName);
	
	[Signal]
	public delegate void RtcOfferReceivedEventHandler(string type, string sdp);
	
	[Signal]
	public delegate void RtcAnswerReceivedEventHandler(string type, string sdp);
	
	[Signal]
	public delegate void RtcIceReceivedEventHandler(string media, int index, string name);

	[Signal]
	public delegate void GameStartReceivedEventHandler(string peers);

	[Signal]
	public delegate void ServerChangedHostEventHandler();
	
	[Signal]
	public delegate void UserNameFeedbackReceivedEventHandler();
	
	[Signal]
	public delegate void ResetConnectionEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (wsPeer.ConnectToUrl(url) != Error.Ok)
		{
			GD.Print("[Error] Could not connect to server!");
		}
	}

	public bool IsConnectionOpen()
	{
		if (wsPeer.GetReadyState() == WebSocketPeer.State.Open)
		{
			GD.Print("[Log] Connection open and ready to use!");
			return true;
		}
		return false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		wsPeer.Poll();
		switch (wsPeer.GetReadyState())
		{
			case WebSocketPeer.State.Open:
				while (wsPeer.GetAvailablePacketCount() > 0)
				{
					ParseMessage();
				}
				break;
			case WebSocketPeer.State.Closed:
				var closeCode = wsPeer.GetCloseCode();
				var closeReason = wsPeer.GetCloseReason();
				GD.Print($"Closed Websocket with code: {closeCode}; and with reason {closeReason}.");
				SetProcess(false);
				connected = false;
				EmitSignal("ResetConnection");
				break;
		}
	}

	private void ParseMessage()
	{
		var packetBuffer = wsPeer.GetPacket().GetStringFromUtf8();
		var packet = JsonSerializer.Deserialize<Packet>(packetBuffer);
		if (packet is null)
		{
			return;
		}

		switch (packet.type)
		{
			case MessageType.UserInfo:
				break;
			case MessageType.LobbyList:
				break;
			case MessageType.CreateLobby:
				break;
			case MessageType.JoinLobby:
				break;
			case MessageType.LeaveLobby:
				break;
			case MessageType.LobbyMessage:
				break;
			case MessageType.Offer:
				break;
			case MessageType.Answer:
				break;
			case MessageType.InteractiveConnectivityEstablishment:
				break;
			case MessageType.StartSession:
				break;
			case MessageType.Host:
				break;
		}
	}
}