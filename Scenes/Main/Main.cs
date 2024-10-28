using Godot;

namespace NightmareNegotiations.Scenes.Main;

public partial class Main : Node3D
{
	[Export] public GameManager GameManager { get; private set; }
	public override void _Ready()
	{
		GameManager = new();
		AddChild(GameManager);
	}
}