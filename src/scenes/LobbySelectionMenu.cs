using Godot;

namespace NightmareNegotiations.scenes;

public partial class LobbySelectionMenu : Control
{
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
        GetParent().AddChild(GD.Load<PackedScene>("res://Scenes/MainMenu/MainMenu.tscn").Instantiate());
        QueueFree();
    }
    
    private void OnRefreshLobbyListButtonPressed()
    {
        
    }
}