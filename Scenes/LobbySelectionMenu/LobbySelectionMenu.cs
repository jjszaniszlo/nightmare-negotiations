using Godot;

namespace NightmareNegotiations.Scenes.LobbySelectionMenu;

public partial class LobbySelectionMenu : Control
{
    [Export] public VBoxContainer LobbyListVBoxContainer { get; private set; }
    
    private LineEdit lobbyCodeTextBox;
    
    [Signal]
    public delegate void OnSelectJoinLobbyEventHandler(ulong lobbyCode);
    
    public override void _Ready()
    {
        lobbyCodeTextBox = GetNode<LineEdit>("LobbyCodeTextBox");
    }

	// button signal terminals
    private void OnJoinButtonPressed()
    {
        OnLobbyCodeTextSubmitted(lobbyCodeTextBox.Text);
    }

    private void OnLobbyCodeTextSubmitted(string text)
    {
        GD.Print($"Joining lobby with code: {text}");
    }
    
    private void OnCreateLobbyButtonPressed()
    {
    }
    
    private void OnBackButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/MainMenu/MainMenu.tscn");
    }
    
    private void OnRefreshLobbyListButtonPressed()
    {
    }
}
