using Godot;

namespace NightmareNegotiations;

public partial class TransformFollower : Node3D
{
	[Export] public Node3D Target { get; private set; }
	[Export] public float FollowRotationSpeed { get; private set; } = 5.0f;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalRotation = GlobalRotation.Lerp(Target.GlobalRotation, (float)delta * FollowRotationSpeed);
	}
}
