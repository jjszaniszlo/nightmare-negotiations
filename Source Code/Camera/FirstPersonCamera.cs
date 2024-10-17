using Godot;

namespace NightmareNegotiations.Camera;

public partial class FirstPersonCamera : Node3D
{
	[Export]
	public Camera3D camera;

	[Export]
	public float cameraSensitivity;
	[Export]
	public float minCameraElevation;
	[Export]
	public float maxCameraElevation;
	
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
			RotateY(-m.Relative.X * cameraSensitivity * 0.01f);
			// rotate the camera, up and down
			camera.RotateX(-m.Relative.Y * cameraSensitivity * 0.01f);

			// clamp the amount in which you can look up and down.
			Vector3 camRotation = camera.Rotation;
			camRotation.X = Mathf.Clamp(
				camRotation.X,
				Mathf.DegToRad(-minCameraElevation), 
				Mathf.DegToRad(maxCameraElevation));
			camera.Rotation = camRotation;
		} else if (@event is InputEventKey k && k.Keycode == Key.Escape)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}