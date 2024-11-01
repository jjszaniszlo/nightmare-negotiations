using Godot;

namespace NightmareNegotiations.Scenes.MainMenu;

public partial class MainMenu : Node
{
    private void OnSinglePlayerButtonPressed()
    {
        var loadingScene = GD.Load<PackedScene>("res://Scenes/LoadingScene/Loading.tscn").Instantiate<LoadingScreen>();
        loadingScene.LoadScene = "res://Scenes/LobbyScene/SingleplayerLobby.tscn";
        AddChild(loadingScene);
    }
    
    private void OnMultiPlayerButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/LobbySelectionMenu/LobbySelectionMenu.tscn");
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}