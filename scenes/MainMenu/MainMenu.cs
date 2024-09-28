using Godot;

namespace NightmareNegotiations.scenes.MainMenu;

public partial class MainMenu : Node
{
    private PackedScene worldTemplate = GD.Load<PackedScene>("res://scenes/world.tscn");
    
    private void OnSinglePlayerButtonPressed()
    {
        GetParent().AddChild(worldTemplate.Instantiate());
        QueueFree();
    }
}