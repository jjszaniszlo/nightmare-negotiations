using Godot;

namespace NightmareNegotiations.Scenes.Main;

public partial class Main : Node3D
{
	[Export] private GameManager gameManager;
	public override void _Ready()
	{
		// AddChild(GD.Load<PackedScene>("res://Scenes/MainMenu/MainMenu.tscn").Instantiate());
	}
}