using System.Collections.Generic;
using System.Text.Json;
using Godot;

namespace NightmareNegotiations.net;

public partial class Client : Node
{
	private WebRtcMultiplayerPeer rtcMulitplayerPeer = new();
	private WebSocketPeer wsPeer = new();
	private string url = "ws://127.0.0.1:4556";
	private bool connected = false;

	[Signal]
	private delegate void InvalidNewLobbyName();
	
	[Signal]
	private delegate void InvalidJoinLobbyName();
	
	[Signal]
	private delegate void NewLobby(string lobbyName);
	
	[Signal]
	private delegate void JoinLobby(string lobbyName);
	
	[Signal]
	private delegate void LobbyListReceived(List<string> lobbyList);
	
	[Signal]
	private delegate void LobbyMessageReceived(string userName, string message);

	[Signal]
	private delegate void AnotherUserJoinedLobby(string anotherUserName);
	
	[Signal]
	private delegate void HostNameReceived(string hostName);
	
	[Signal]
	private delegate void AnotherUserLeftLobby(string anotherUserName);
	
	[Signal]
	private delegate void RtcOfferReceived(string type, string sdp);
	
	[Signal]
	private delegate void RtcAnswerReceived(string type, string sdp);
	
	[Signal]
	private delegate void RtcIceReceived(string media, int index, string name);

	[Signal]
	private delegate void GameStartReceived(string peers);

	[Signal]
	private delegate void ServerChangedHost();
	
	[Signal]
	private delegate void UserNameFeedbackReceived();
	
	[Signal]
	private delegate void ResetConnection();
	
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