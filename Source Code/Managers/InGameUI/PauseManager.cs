using Godot;
using NightmareNegotiations.Scenes.PauseMenu;

namespace NightmareNegotiations;

public partial class PauseManager : Node, IGameInterfaceManager
{
	private bool pauseState;
	[Export] public CanvasLayer PostProcessing { get; private set; }
	[Export] public Player Player { get; private set; }
	
	[Signal]
	public delegate void OnQuitGameEventHandler();
	
	private PauseMenu pauseMenu;
	private DelegateOnZeroCounter interfaceDelegateOnZeroCounter;

	public override void _Ready()
	{
		pauseMenu = GetNode<PauseMenu>("PauseMenu");
		pauseMenu.OnSelectResume += HandlePause;
		pauseMenu.OnSelectQuit += () => EmitSignal(SignalName.OnQuitGame);
		
		interfaceDelegateOnZeroCounter = GetParent()
			.GetNode<DelegateOnZeroCounter>("InterfaceOnZeroCounter");
		interfaceDelegateOnZeroCounter.OnZero += DisableRequired;
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
			EnableRequired();
			interfaceDelegateOnZeroCounter.Add();
		}
		else
		{
			pauseMenu.Visible = false;
			interfaceDelegateOnZeroCounter.Subtract();
		}
	}
	
	public void EnableRequired()
	{
		Player.SetPhysicsProcess(false);
		Player.GetNode("Head").SetProcessInput(false);
		PostProcessing.Visible = false;
		Input.SetMouseMode(Input.MouseModeEnum.Visible);
	}

	public void DisableRequired()
	{
		Player.SetPhysicsProcess(true);
		Player.GetNode("Head").SetProcessInput(true);
		PostProcessing.Visible = true;
		Input.SetMouseMode(Input.MouseModeEnum.Captured);
	}
}