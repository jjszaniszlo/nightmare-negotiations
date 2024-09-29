using System;
using System.Linq;
using System.Text.Json;
using Godot;
using Newtonsoft.Json;
using NightmareNegotiations.scenes;

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
	public delegate void NewLobbyEventHandler(string lobbyCode, string lobbyDescription);
	
	[Signal]
	public delegate void JoinLobbyEventHandler(string lobbyCode);
	
	[Signal]
	public delegate void LobbyListReceivedEventHandler(string[] lobbyCodes, string[] lobbyDescriptions);
	
	[Signal]
	public delegate void LobbyMessageReceivedEventHandler(string userName, string message);
	
	[Signal]
	public delegate void HostNameReceivedEventHandler(string hostName);
	
	[Signal]
	public delegate void AnotherUserJoinedLobbyEventHandler(string anotherUserName);
	
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
				Main.Globals.User.UserName = packet.data;
				Main.Globals.User.Id = packet.id;
				
				GD.Print($"Received User: {packet.data}:{packet.id}");
				EmitSignal(SignalName.UserNameFeedbackReceived);
				connected = true;
				break;
			case MessageType.LobbyList:
				if (packet.data == "")
				{
					EmitSignal(SignalName.LobbyListReceived, System.Array.Empty<string>());
				}
				else
				{
					var lobbyList = packet.data.Split("@@@");
					EmitSignal(SignalName.LobbyListReceived, lobbyList);
				}
				break;
			case MessageType.CreateLobby:
				if (packet.data == "INVALID")
				{
					EmitSignal(SignalName.InvalidNewLobbyName);
				}
				else
				{
					var splitData = packet.data.Split("@@@", 2);
					var lobbyCode = splitData[0];
					var lobbyDescription = splitData[1];
					EmitSignal(SignalName.NewLobby, lobbyCode, lobbyDescription);
				}
				break;
			case MessageType.JoinLobby:
			{
				var splitData = packet.data.Split("@@@");
				var header = splitData[0];
				var payload = splitData[1];
				if (header == "INVALID")
				{
					EmitSignal(SignalName.InvalidJoinLobbyName);
				}
				else if (header == "LOBBY_CODE")
				{
					EmitSignal(SignalName.JoinLobby, payload);
				}
				else if (header == "NEW_PLAYER_JOINED_NAME")
				{
					Main.Globals.User.Peers.Add(packet.id, payload);
					EmitSignal(SignalName.AnotherUserJoinedLobby, payload);
				}
				else if (header == "EXISTING_PLAYER_JOINED_NAME")
				{
					Main.Globals.User.Peers.Add(packet.id, payload);
					EmitSignal(SignalName.AnotherUserJoinedLobby, payload);
				}
				break;
			}
			case MessageType.LeaveLobby:
				break;
			case MessageType.LobbyMessage:
				break;
			case MessageType.Offer:
			{
				var splitData = packet.data.Split("@@@", 2);
				var type = splitData[0];
				var sdp = splitData[1];

				EmitSignal(SignalName.RtcOfferReceived, type, sdp, packet.id);
				break;
			}
			case MessageType.Answer:
			{
				var splitData = packet.data.Split("@@@", 2);
				var type = splitData[0];
				var sdp = splitData[1];

				EmitSignal(SignalName.RtcAnswerReceived, type, sdp, packet.id);
				break;
			}
			case MessageType.InteractiveConnectivityEstablishment:
			{
				var splitData = packet.data.Split("@@@", 3);
				var media = splitData[0];
				var index = splitData[1].ToInt();
				var name = splitData[2];

				EmitSignal(SignalName.RtcIceReceived, media, index, name, packet.id);
				break;
			}
			case MessageType.StartSession:
				break;
			case MessageType.Host:
				// assign User to host if they are hosting the lobby, otherwise assign their HostName property to the current
				// host of the lobby.
				if (packet.id == Main.Globals.User.Id && packet.data == Main.Globals.User.UserName)
				{
					Main.Globals.User.IsHost = true;
				}
				else
				{
					Main.Globals.User.IsHost = false;
					Main.Globals.User.HostUserName = packet.data;
				}

				EmitSignal(SignalName.ServerChangedHost);
				break;
		}
	}
}