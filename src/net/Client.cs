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
	
	public Client()
	{
		if (wsPeer.ConnectToUrl(url) == Error.Ok)
		{
			GD.Print("[Log] Connected to server!");
		}
		else
		{
			GD.Print("[Error] Could not connect to server!");
		}
	}

	public bool IsConnectionOpen()
	{
		return wsPeer.GetReadyState() == WebSocketPeer.State.Open;
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
		var packet = JsonConvert.DeserializeObject<BasicPacket>(packetBuffer, new JsonPacketConverter());
		if (packet is null)
		{
			return;
		}
		if (packet is UserInfoPacket userInfoPacket)
		{
			Main.Globals.User.UserName = userInfoPacket.Username;
			Main.Globals.User.Id = userInfoPacket.Id;
			
			GD.Print($"[Log] Received User from Server: {userInfoPacket.Username}:{userInfoPacket.Id}");
			EmitSignal(SignalName.UserNameFeedbackReceived);
			connected = true;
		}
		else if (packet is LobbyListReceivePacket lobbyListPacket)
		{
			if (lobbyListPacket.LobbyList.Count == 0)
			{
				GD.Print("[Log] No available lobbies.");
				var lobbyCodes = new[]{"XXXAVL", "HJCLAD"};
				var lobbyDescriptions = new[]{"Cool lobby 1", "Epic Gamers 2"};
				EmitSignal(SignalName.LobbyListReceived, lobbyCodes, lobbyDescriptions);
			}
			else
			{
				var lobbyCodes = lobbyListPacket.LobbyList.Select(lobby => lobby.LobbyCode).ToArray();
				var lobbyDescriptions = lobbyListPacket.LobbyList.Select(lobby => lobby.LobbyDescription).ToArray();
				GD.Print($"[Log] {lobbyCodes.Length} available lobbies.");
				EmitSignal(SignalName.LobbyListReceived, lobbyCodes, lobbyDescriptions);
			}
		}
		// 	case Message.CreateLobby:
		// 		if (packet.data == "INVALID")
		// 		{
		// 			EmitSignal(SignalName.InvalidNewLobbyName);
		// 		}
		// 		else
		// 		{
		// 			var splitData = packet.data.Split("@@@", 2);
		// 			var lobbyCode = splitData[0];
		// 			var lobbyDescription = splitData[1];
		// 			EmitSignal(SignalName.NewLobby, lobbyCode, lobbyDescription);
		// 		}
		// 		break;
		// 	case Message.JoinLobby:
		// 	{
		// 		var splitData = packet.data.Split("@@@");
		// 		var header = splitData[0];
		// 		var payload = splitData[1];
		// 		if (header == "INVALID")
		// 		{
		// 			EmitSignal(SignalName.InvalidJoinLobbyName);
		// 		}
		// 		else if (header == "LOBBY_CODE")
		// 		{
		// 			EmitSignal(SignalName.JoinLobby, payload);
		// 		}
		// 		else if (header == "NEW_PLAYER_JOINED_NAME")
		// 		{
		// 			Main.Globals.User.Peers.Add(packet.id, payload);
		// 			EmitSignal(SignalName.AnotherUserJoinedLobby, payload);
		// 		}
		// 		else if (header == "EXISTING_PLAYER_JOINED_NAME")
		// 		{
		// 			Main.Globals.User.Peers.Add(packet.id, payload);
		// 			EmitSignal(SignalName.AnotherUserJoinedLobby, payload);
		// 		}
		// 		break;
		// 	}
		// 	case Message.LeaveLobby:
		// 		break;
		// 	case Message.LobbyMessage:
		// 		break;
		// 	case Message.Offer:
		// 	{
		// 		var splitData = packet.data.Split("@@@", 2);
		// 		var type = splitData[0];
		// 		var sdp = splitData[1];
		//
		// 		EmitSignal(SignalName.RtcOfferReceived, type, sdp, packet.id);
		// 		break;
		// 	}
		// 	case Message.Answer:
		// 	{
		// 		var splitData = packet.data.Split("@@@", 2);
		// 		var type = splitData[0];
		// 		var sdp = splitData[1];
		//
		// 		EmitSignal(SignalName.RtcAnswerReceived, type, sdp, packet.id);
		// 		break;
		// 	}
		// 	case Message.InteractiveConnectivityEstablishment:
		// 	{
		// 		var splitData = packet.data.Split("@@@", 3);
		// 		var media = splitData[0];
		// 		var index = splitData[1].ToInt();
		// 		var name = splitData[2];
		//
		// 		EmitSignal(SignalName.RtcIceReceived, media, index, name, packet.id);
		// 		break;
		// 	}
		// 	case Message.StartSession:
		// 		break;
		// 	case Message.Host:
		// 		// assign User to host if they are hosting the lobby, otherwise assign their HostName property to the current
		// 		// host of the lobby.
		// 		if (packet.id == Main.Globals.User.Id && packet.data == Main.Globals.User.UserName)
		// 		{
		// 			Main.Globals.User.IsHost = true;
		// 		}
		// 		else
		// 		{
		// 			Main.Globals.User.IsHost = false;
		// 			Main.Globals.User.HostUserName = packet.data;
		// 		}
		//
		// 		EmitSignal(SignalName.ServerChangedHost);
		// 		break;
		// }
	}

	private void SendPacket(BasicPacket packet)
	{
		wsPeer.SendText(JsonConvert.SerializeObject(packet));
	}

	public void SendUserName(string username)
	{
		SendPacket(new UserInfoPacket
		{
			Id = 0,
			Username = username
		});
	}

	public void RequestLobbyList()
	{
		SendPacket(new BasicPacket
		{
			Message = Message.LobbyList,
			Id = Main.Globals.User.Id,
		});
	}

	public void RequestJoinLobby(string lobbyCode)
	{
		GD.Print($"[Log] Requesting lobby {lobbyCode}");
	}
}