using Godot;
using Steam;

namespace NightmareNegotiations.Scenes.Main;

public partial class Main : Node3D
{
	public static Main Instance { get; private set; }
	public SteamManager SteamManager { get; private set; } = new();
	[Export] public GameManager GameManager { get; private set; }
	public override void _Ready()
	{
		Instance = this;
		
		SteamManager.SteamAppId = 480;
		SteamManager._Ready();
		
		GameManager = new();
		AddChild(GameManager);
	}
}