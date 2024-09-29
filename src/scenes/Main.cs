using Godot;

namespace NightmareNegotiations.scenes;

public partial class Main : Node3D
{
	public static Globals Globals { get; private set; } = new();
	private PackedScene usernamePromptTemplate = GD.Load<PackedScene>("res://Scenes/UsernamePrompt/UsernamePrompt.tscn");
	
	public override void _Ready()
	{
		AddChild(usernamePromptTemplate.Instantiate());
	}
}