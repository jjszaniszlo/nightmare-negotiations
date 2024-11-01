using Godot;

namespace NightmareNegotiations;

public partial class FirstPersonCamera : Node3D
{
	[Export]
	public Camera3D Camera { get; private set; }
	[Export]
	public float CameraSensitivity { get; private set; }
	[Export]
	public float MinCameraElevation { get; private set; }
	[Export]
	public float MaxCameraElevation { get; private set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.SetMouseMode(Input.MouseModeEnum.Captured);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion m)
		{
			// rotate the head itself, left and right
			RotateY(-m.Relative.X * CameraSensitivity * 0.01f);
			// rotate the camera, up and down
			Camera.RotateX(-m.Relative.Y * CameraSensitivity * 0.01f);

			// clamp the amount in which you can look up and down.
			Vector3 camRotation = Camera.Rotation;
			camRotation.X = Mathf.Clamp(
				camRotation.X,
				Mathf.DegToRad(MinCameraElevation), 
				Mathf.DegToRad(MaxCameraElevation));
			Camera.Rotation = camRotation;
		}
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
