using Godot;
using NightmareNegotiations.net;

namespace NightmareNegotiations.scenes;

public partial class Main : Node3D
{
	private PackedScene usernamePromptTemplate;
	
	public override void _Ready()
	{
		if (OS.HasFeature("dedicated_server"))
		{
			NetworkManager.Instance.InitDedicatedServer();
		}
		else
		{
			NetworkManager.Instance.InitClient();
		}
		
		usernamePromptTemplate = GD.Load<PackedScene>("res://Scenes/UsernamePrompt/UsernamePrompt.tscn");
	}
}