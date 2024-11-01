using Godot;

namespace NightmareNegotiations.Scenes.MainMenu;

public partial class MainMenu : Node
{
    private void OnSinglePlayerButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/Lobby/SingleplayerLobby.tscn");
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