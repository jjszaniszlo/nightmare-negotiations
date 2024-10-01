using Godot;

namespace NightmareNegotiations.scenes.LobbySelectionMenu;

public partial class LobbySelectionMenu : Control
{
    private PackedScene mainMenuTemplate = GD.Load<PackedScene>("res://scenes/MainMenu/MainMenu.tscn");
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
}