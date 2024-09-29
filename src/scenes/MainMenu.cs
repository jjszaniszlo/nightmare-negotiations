using Godot;

namespace NightmareNegotiations.scenes;

public partial class MainMenu : Node
{
    private PackedScene worldTemplate = GD.Load<PackedScene>("res://Scenes/World/World.tscn");
    private PackedScene lobbyMenuTemplate =
        GD.Load<PackedScene>("res://Scenes/LobbySelectionMenu/LobbySelectionMenu.tscn");

    public override void _Ready()
    {
        GetNode<Label>("Connection Status/Online_Offline").Text = Main.Globals.User.Client.IsConnectionOpen()
            ? "Online"
            : "Offline";
    }

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