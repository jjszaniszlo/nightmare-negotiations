using Godot;

namespace NightmareNegotiations.Scenes.LobbySelectionMenu;

public partial class LobbySelectionMenu : Control
{
    [Export] public VBoxContainer LobbyListVBoxContainer { get; private set; }
    [Export] public LineEdit LobbyCodeTextBox { get; private set; }

    private LobbyManager lobbyManager;
    
    [Signal]
    public delegate void OnSelectJoinLobbyEventHandler(ulong lobbyCode);
    
    public override void _Ready()
    {
        lobbyManager = new();
        AddChild(lobbyManager);

        lobbyManager.CreateLobbyScene += OnCreateLobbyScene;
        
        LobbyCodeTextBox = GetNode<LineEdit>("LobbyCodeTextBox");
    }

	// button signal terminals
    private void OnJoinButtonPressed()
    {
        OnLobbyCodeTextSubmitted(LobbyCodeTextBox.Text);
    }

    private void OnLobbyCodeTextSubmitted(string text)
    {
        if (ulong.TryParse(text, out var lobbyId))
        {
            GD.Print($"Joining lobby with code: {lobbyId}");
            lobbyManager.JoinLobby(lobbyId);
        }
        else
        {
            GD.Print("Could not join lobby! Invalid lobby code consisting of non integer characters!");
        }
    }
    
    private async void OnCreateLobbyButtonPressed()
    {
        await Globals.Instance.SteamManager.CreateLobby();
    }

    private void OnCreateLobbyScene(long peerId)
    {
        var lobby = GD.Load<PackedScene>("res://Scenes/LobbyScene/MultiplayerLobby.tscn").Instantiate<LobbyScene>();
        lobby.AddPlayer(peerId);
        NetworkUser.Instance.AddChild(lobby);
        QueueFree();
    }
    
    private void OnBackButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/MainMenu/MainMenu.tscn");
    }
    
    private void OnRefreshLobbyListButtonPressed()
    {
    }
}
