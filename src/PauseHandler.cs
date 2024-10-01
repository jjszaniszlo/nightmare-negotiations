using Godot;
using System;

public partial class PauseHandler : Node
{
	private bool pauseState = false;
	
	[Signal]
	public delegate void OnPauseEventHandler(bool shouldPause);
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("game_pause"))
		{
			EmitSignal(SignalName.OnPause, !pauseState);
			pauseState = !pauseState;
		}
	}
}
