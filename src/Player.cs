using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[ExportCategory("Player Movement Options")]
	
	[Export]
	private float speed = 5.0f;
	[Export]
	private float jumpVelocity = 4.5f;
	
	[Export]
	private bool shouldFollowDirection;
	[Export]
	private Node3D followDirectionParent;

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = jumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector(
			"move_left",
			"move_right",
			"move_forward",
			"move_backward");
		Vector3 direction = (
			(shouldFollowDirection 
				? followDirectionParent.GlobalTransform.Basis
				: Transform.Basis)
			* new Vector3(inputDir.X, 0, inputDir.Y)).Normalized(); 
		
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
