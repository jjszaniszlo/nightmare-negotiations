using Godot;

namespace NightmareNegotiations.Scenes.LobbySelectionMenu;

public partial class LobbySelectionMenu : Control
{
    
    [Signal]
    public delegate void OnSelectBackEventHandler();
    
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
        EmitSignal(SignalName.OnSelectBack);
    }
    
    private void OnRefreshLobbyListButtonPressed()
    {
        
    }
}