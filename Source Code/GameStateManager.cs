using Godot;
using NightmareNegotiations.Utils;

namespace NightmareNegotiations;

public partial class GameStateManager : Node
{
	private StateMachine state = new();

	public GameStateManager()
	{
		state.AddState(GameState.Menu, "Menu");
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private enum GameState : uint {
		Menu,
		InLobby,
		InGame,
	}
}