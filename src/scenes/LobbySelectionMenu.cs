using Godot;

namespace NightmareNegotiations.scenes;

public partial class LobbySelectionMenu : Control
{
    private PackedScene mainMenuTemplate = GD.Load<PackedScene>("res://Scenes/MainMenu/MainMenu.tscn");
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