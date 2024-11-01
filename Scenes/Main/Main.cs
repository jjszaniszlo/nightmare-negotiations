using Godot;
using Steam;

namespace NightmareNegotiations.Scenes.Main;

public partial class Main : Node3D
{
	public const uint APP_ID = 3328430;
	public static Main Instance { get; private set; }
	public SteamManager SteamManager { get; private set; } = new();

	public GameManager GameManager { get; private set; }

	public override void _Ready()
	{
		Instance = this;
		
		OS.SetEnvironment("SteamAppId", APP_ID.ToString());
		OS.SetEnvironment("SteamGameId", APP_ID.ToString());
		
		SteamManager.SteamAppId = APP_ID;
		SteamManager._Ready();
		
		GameManager = new();
		AddChild(GameManager);
	}
}
