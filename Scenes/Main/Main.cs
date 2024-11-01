using Godot;
using Steam;

namespace NightmareNegotiations.Scenes.Main;

public partial class Main : Node3D
{
	public static Main Instance { get; private set; }
	public override void _Ready()
	{
		Instance = this;
	}
}
