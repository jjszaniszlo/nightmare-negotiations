using Godot;
using Steam;

namespace NightmareNegotiations;

public partial class Globals : Node
{
	public static Globals Instance;
	
	public const uint APP_ID = 480;
	
	public SteamManager SteamManager { get; private set; } = new();

	public Globals()
	{
		Instance = this;
		
		OS.SetEnvironment("SteamAppId", APP_ID.ToString());
		OS.SetEnvironment("SteamGameId", APP_ID.ToString());
		
		SteamManager.SteamAppId = APP_ID;
		SteamManager._Ready();
	}
}
