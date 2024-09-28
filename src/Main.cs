using Godot;

namespace NightmareNegotiations;

public partial class Main : Node3D
{
	private PackedScene mainMenuTemplate = GD.Load<PackedScene>("res://scenes/MainMenu/MainMenu.tscn");
	
	public override void _Ready()
	{
		AddChild(mainMenuTemplate.Instantiate());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}