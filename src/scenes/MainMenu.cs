using Godot;

namespace NightmareNegotiations.scenes;

public partial class MainMenu : Node
{
    private PackedScene worldTemplate = GD.Load<PackedScene>("res://Scenes/World/World.tscn");

    private PackedScene lobbyMenuTemplate =
        GD.Load<PackedScene>("res://scenes/LobbySelectionMenu/LobbySelectionMenu.tscn");
    
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