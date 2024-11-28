using System.Collections.Generic;
using Godot;
using Steam;
using Steamworks;
using Steamworks.Data;

namespace NightmareNegotiations.Scenes.LobbySelectionMenu;

public partial class LobbySelectionManager : Node
{
    [Signal] public delegate void CreateLobbySceneEventHandler(long peerId);
    
    public override void _Ready()
    {
        Globals.Instance.SteamManager.OnLobbySuccessfullyCreated += OnLobbySucessfullyCreated;
        Globals.Instance.SteamManager.OnLobbyRefreshCompleted += OnLobbyRefreshCompleted;
    }

    private void OnLobbyRefreshCompleted(List<Lobby> lobbyList)
    {
    }

    public async void JoinLobby(SteamId lobbyId)
    {
        var lobby = new Lobby(lobbyId);
        var result = await lobby.Join();
        if (result != RoomEnter.Success)
        {
            GD.Print($"Could not join lobby! {result}");
            return;
        }

        var steamMultiplayerPeer = new SteamMultiplayerPeer();
        steamMultiplayerPeer.CreateClient(Globals.Instance.SteamManager.PlayerSteamID, lobby.Owner.Id);

        NetworkUser.Instance.Multiplayer.MultiplayerPeer = steamMultiplayerPeer;
        NetworkUser.Instance.PeerId = steamMultiplayerPeer.GetUniqueId();
        NetworkUser.Instance.InLobby = true;
        NetworkUser.Instance.LobbyId = lobby.Id.Value;
        
        EmitSignal(SignalName.CreateLobbyScene, steamMultiplayerPeer.GetUniqueId());
    }

    public void OnLobbySucessfullyCreated(Lobby lobby)
    {
        // create host multiplayer peer
        var steamMultiplayerPeer = new SteamMultiplayerPeer();
        steamMultiplayerPeer.CreateHost(25565);
        
        NetworkUser.Instance.Multiplayer.MultiplayerPeer = steamMultiplayerPeer;
        NetworkUser.Instance.PeerId = steamMultiplayerPeer.GetUniqueId();
        NetworkUser.Instance.InLobby = true;
        NetworkUser.Instance.LobbyId = lobby.Id.Value;
        
        GD.Print($"Lobby created with code: {lobby.Id}");

        EmitSignal(SignalName.CreateLobbyScene, steamMultiplayerPeer.GetUniqueId());
    }
}