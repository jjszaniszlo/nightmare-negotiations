using Godot;

namespace NightmareNegotiations.Scenes.LobbySelectionMenu;

public partial class LobbySelectionMenu : Control
{
    [Export] public VBoxContainer LobbyListVBoxContainer { get; private set; }
    
    private LineEdit lobbyCodeTextBox;
    
    [Signal]
    public delegate void OnSelectBackEventHandler();

    [Signal]
    public delegate void OnSelectJoinLobbyEventHandler(ulong lobbyCode);
    
    [Signal]
    public delegate void OnSelectHostLobbyEventHandler();
    
    [Signal]
    public delegate void OnSelectRefreshLobbyListEventHandler();

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
        if (ulong.TryParse(text, out var lobbySteamId))
        {
            EmitSignal(SignalName.OnSelectJoinLobby, lobbySteamId);
        }
        else
        {
            GD.PrintErr("Invalid lobby code format!");
        }
    }
    
    private void OnCreateLobbyButtonPressed()
    {
        EmitSignal(SignalName.OnSelectHostLobby);
    }
    
    private void OnBackButtonPressed()
    {
        EmitSignal(SignalName.OnSelectBack);
    }
    
    private void OnRefreshLobbyListButtonPressed()
    {
        EmitSignal(SignalName.OnSelectRefreshLobbyList);
    }
}
