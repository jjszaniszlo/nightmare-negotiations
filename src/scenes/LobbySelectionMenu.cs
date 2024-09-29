using Godot;
using NightmareNegotiations.net;

namespace NightmareNegotiations.scenes;

public partial class LobbySelectionMenu : Control
{
    private PackedScene mainMenuTemplate = GD.Load<PackedScene>("res://Scenes/MainMenu/MainMenu.tscn");

    public LobbySelectionMenu()
    {
        Main.Globals.User.Client.RequestLobbyList();
        Main.Globals.User.Client.LobbyListReceived += CreateLobbyList;
    }

    private void OnJoinButtonPressed()
    {
        
    }

    private void OnLobbyCodeTextSubmitted(string text)
    {
        OnJoinButtonPressed();
    }
    
    private void OnCreateLobbyButtonPressed()
    {
    }
    
    private void OnBackButtonPressed()
    {
        GetParent().AddChild(mainMenuTemplate.Instantiate());
        QueueFree();
    }
    
    private void OnRefreshLobbyListButtonPressed()
    {
        
    }

    private void CreateLobbyList(string[] lobbyCodes, string[] lobbyDescriptions)
    {
        if (lobbyCodes.Length == 0 || lobbyDescriptions.Length == 0) return;
        
        var vboxContainer = GetNode<VBoxContainer>("PublicLobbyListContainer/LobbyList/Container");
            
        // clear scroll container nodes.
        foreach (var node in vboxContainer.GetChildren()) node?.QueueFree();

        // add all lobbies found to it.
        for (var i = 0; i < lobbyCodes.Length; i++)
        {
            var lobbyEntry = (Control)GetNode<Control>("AvailableLobbyTemplate").Duplicate();
            lobbyEntry.Show();
            lobbyEntry.GetChild<Label>(0).Text = lobbyDescriptions[i];
            lobbyEntry.GetChild<Label>(1).Text = lobbyCodes[i];
            lobbyEntry.GetChild<Button>(2).Pressed += () =>
                OnJoinLobbyPressed(lobbyEntry.GetChild<Label>(1).Text);
            vboxContainer.AddChild(lobbyEntry);
        }
    }

    private void OnJoinLobbyPressed(string lobbyCode)
    {
        Main.Globals.User.Client.RequestJoinLobby(lobbyCode);
    }
}