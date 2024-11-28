using Godot;

public partial class CreatureMovement : CharacterBody3D
{
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
	
		Velocity = velocity;
		MoveAndSlide();
	}
}
