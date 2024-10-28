using Godot;
using NightmareNegotiations.Scenes.Lobby;

namespace NightmareNegotiations;

public partial class LobbyComputerManager : Node, IGameInterfaceManager
{
	[Export] public CanvasLayer PostProcessing { get; private set; }
	[Export] public Player Player { get; private set; }
	[Export] public ComputerInteractable Computer { get; private set; }

	private bool state;
	private ComputerInterface computerInterface;
	private DelegateOnZeroCounter interfaceDelegateOnZeroCounter;

	public override void _Ready()
	{
		Computer.OnInteract += HandleInteract;
		computerInterface = GetNode<ComputerInterface>("ComputerInterface");

		interfaceDelegateOnZeroCounter = GetParent()
			.GetNode<DelegateOnZeroCounter>("InterfaceOnZeroCounter");
		interfaceDelegateOnZeroCounter.OnZero += DisableRequired;
	}

	private void HandleInteract()
	{
		state = !state;
		if (state)
		{
			EnableRequired();
			computerInterface.Visible = true;
			interfaceDelegateOnZeroCounter.Add();
		}
		else
		{
			computerInterface.Visible = false;
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