using System.Collections.Generic;
using Godot;
using NightmareNegotiations.Scenes.LobbySelectionMenu;
using NightmareNegotiations.Scenes.Main;
using Steamworks;
using Steamworks.Data;

namespace NightmareNegotiations;

public partial class LobbyManager : Node
{
    private LobbySelectionMenu lobbySelectionMenu;
    
    public override void _Ready()
    {
        lobbySelectionMenu = GetParent<LobbySelectionMenu>();

        if (lobbySelectionMenu == null)
        {
            GD.PrintErr("LobbyManager must be a node child of lobby selection menu!");
            return;
        }

        lobbySelectionMenu.OnSelectJoinLobby += OnJoinLobbyButtonSelected;
        lobbySelectionMenu.OnSelectHostLobby += OnHostLobbyButtonSelected;
        lobbySelectionMenu.OnSelectRefreshLobbyList += OnRefreshLobbyListButtonSelected;

        Main.Instance.SteamManager.OnLobbyRefreshCompleted += OnLobbyRefreshCompleted;
    }

    private void OnLobbyRefreshCompleted(List<Lobby> lobbies)
    {
    }

    private void OnRefreshLobbyListButtonSelected()
    {
    }

    private void OnHostLobbyButtonSelected()
    {
    }

    private async void OnJoinLobbyButtonSelected(ulong lobbyCode)
    {
        var lobby = new Lobby(lobbyCode);
        var result = await lobby.Join();
        if (result != RoomEnter.Success)
        {
            GD.Print($"Could not join lobby with code {lobbyCode}! ({result})");
            return;
        }
        GD.Print($"Lobby owner name: {lobby.Owner.Name}");
    }
}
