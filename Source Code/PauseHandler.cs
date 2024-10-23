using Godot;
using NightmareNegotiations.Scenes.PauseMenu;

namespace NightmareNegotiations;

public partial class PauseHandler : Node
{
	private bool pauseState;
	[Export] private CanvasLayer postProcessing;
	[Export] private PauseMenu pauseMenu;
	[Export] private Player player;
	
	[Signal]
	public delegate void OnPauseEventHandler(bool shouldPause);

	[Signal]
	public delegate void OnQuitGameEventHandler();

	public override void _Ready()
	{
		pauseMenu.OnSelectResume += HandlePause;
		pauseMenu.OnSelectQuit += () => EmitSignal(SignalName.OnQuitGame);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("game_pause"))
		{
			HandlePause();
		}
	}

	private void HandlePause()
	{
		pauseState = !pauseState;
		
		if (pauseState)
		{
			pauseMenu.Visible = true;
			player.SetPhysicsProcess(false);
			player.GetNode("Head").SetProcessInput(false);
			postProcessing.Visible = false;
			Input.SetMouseMode(Input.MouseModeEnum.Visible);
		}
		else
		{
			pauseMenu.Visible = false;
			player.SetPhysicsProcess(true);
			player.GetNode("Head").SetProcessInput(true);
			postProcessing.Visible = true;
			Input.SetMouseMode(Input.MouseModeEnum.Captured);
		}
	}
}