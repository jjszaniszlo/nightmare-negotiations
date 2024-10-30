using Godot;
using Steam;

namespace NightmareNegotiations.Scenes.Main;

public partial class Main : Node3D
{
	public SteamManager SteamManager { get; private set; } = new();
	[Export] public GameManager GameManager { get; private set; }
	public override void _Ready()
	{
		SteamManager.SteamAppId = 480;
		SteamManager._Ready();
		
		GameManager = new();
		AddChild(GameManager);
	}
}