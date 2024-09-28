using Godot;

namespace NightmareNegotiations.scenes.Main;

public partial class Main : Node3D
{
	public Globals Globals { get; private set; } = new();
	private PackedScene usernamePromptTemplate = GD.Load<PackedScene>("res://scenes/UsernamePrompt/UsernamePrompt.tscn");
	
	public override void _Ready()
	{
		AddChild(usernamePromptTemplate.Instantiate());
	}
}