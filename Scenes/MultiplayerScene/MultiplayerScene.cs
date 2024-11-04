using Godot;
using System;

namespace NightmareNegotiations;

public partial class MultiplayerScene : Node3D
{
	[Export] public Node3D MultiplayerInstances { get; private set; }

	private AudioController audioController = new();
	private CreatureController creatureController = new();
	private LevelController levelController = new();
	private UserInterfaceController userInterfaceController = new();

	public override void _Ready()
	{
		// every peer should have these nodes, because they are either clientside, or a mechanic that should
		// be able to be done by everyone.
		AddChild(audioController);
		AddChild(levelController);
		AddChild(userInterfaceController);
		
		// only hosts should create the factories for creatures
		if (NetworkUser.Instance.IsHost())
		{
			AddChild(creatureController);
		}
	}
}
