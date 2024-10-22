using Godot;

namespace NightmareNegotiations.Scenes.MainMenu;

public partial class MainMenu : Node
{
    private PackedScene worldTemplate = GD.Load<PackedScene>("res://Scenes/Map1.tscn");

    private PackedScene lobbyMenuTemplate =
        GD.Load<PackedScene>("res://Scenes/LobbySelectionMenu/LobbySelectionMenu.tscn");
    
    private void OnSinglePlayerButtonPressed()
    {
        GetParent().AddChild(worldTemplate.Instantiate());
        QueueFree();
    }
    
    private void OnMultiPlayerButtonPressed()
    {
        GetParent().AddChild(lobbyMenuTemplate.Instantiate());
        QueueFree();
    }
}