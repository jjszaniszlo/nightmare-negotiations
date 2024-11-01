using System.Collections.Generic;
using Godot;
using NightmareNegotiations.Scenes.LobbySelectionMenu;
using NightmareNegotiations.Scenes.Main;
using Steam;
using Steamworks;
using Steamworks.Data;

namespace NightmareNegotiations;

public partial class LobbyManager : Node
{
    private LobbySelectionMenu lobbySelectionMenu;

	private bool inLobby;

	[Signal] public delegate void AddPlayerEventHandler(long id);
	[Signal] public delegate void RemovePlayerEventHandler(long id);
	[Signal] public delegate void TransitionLobbySceneEventHandler();
    
    public override void _Ready()
    {
        Main.Instance.SteamManager.OnLobbyRefreshCompleted += OnLobbyRefreshCompleted;
    }

    private void OnLobbyRefreshCompleted(List<Lobby> lobbies)
    {
    }

    public void OnRefreshLobbyListButtonSelected()
    {
    }

    public async void OnHostLobbyButtonSelected()
    {
		Main.Instance.SteamManager.OnLobbySuccessfullyCreated += async (lobby) =>
		{
			var steamMultiplayerPeer = new SteamMultiplayerPeer();
			steamMultiplayerPeer.CreateHost(25565);

			Multiplayer.MultiplayerPeer = steamMultiplayerPeer;
			
			GD.Print($"Lobby Code: {lobby.Id}");

			EmitSignal(SignalName.TransitionLobbyScene);
			await ToSignal(GetTree().CreateTimer(3), Timer.SignalName.Timeout);
			EmitSignal(SignalName.AddPlayer, 1);
		};

		Multiplayer.PeerConnected += (peer) =>
		{	
			EmitSignal(SignalName.AddPlayer, peer);
		};

		Multiplayer.PeerDisconnected += (peer) =>
		{	
			EmitSignal(SignalName.RemovePlayer, peer);
		};

		inLobby = true;
		await Main.Instance.SteamManager.CreateLobby();
    }

    public async void OnJoinLobbyButtonSelected(ulong lobbyCode)
    {
        var lobby = new Lobby(lobbyCode);
        var result = await lobby.Join();
        if (result != RoomEnter.Success)
        {
            GD.Print($"Could not join lobby with code {lobbyCode}! ({result})");
            return;
        }

        var peer = new SteamMultiplayerPeer();
        peer.CreateClient(Main.Instance.SteamManager.PlayerSteamID, lobby.Owner.Id);
        Multiplayer.MultiplayerPeer = peer;

	    EmitSignal(SignalName.TransitionLobbyScene);
    }
}
