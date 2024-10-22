using System;
using Godot;
using NightmareNegotiations.Scenes.LobbySelectionMenu;
using NightmareNegotiations.Scenes.MainMenu;
using NightmareNegotiations.Utils;

namespace NightmareNegotiations;

public partial class GameManager : Node
{
	[Export] public Node menusNode;
	[Export] public Node gameplayNode;
	
	private StateMachine state = new();

	public override void _Ready()
	{
		state.AddState(GameState.MainMenu, "MainMenu");
		state.AddState(GameState.LobbyMenu, "LobbyMenu");
		state.AddState(GameState.Lobby, "Lobby");
		state.AddState(GameState.Game, "Game");
		
		state.Connect("OnMainMenuEntered", Callable.From(OnMainMenuEntered));
		state.Connect("OnLobbyMenuEntered", Callable.From(OnLobbyMenuEntered));
		state.Connect("OnLobbyEntered", Callable.From(OnLobbyEntered));
		state.Connect("OnGameEntered", Callable.From(OnGameEntered));
		
		state.Connect("OnMainMenuLeft", Callable.From(OnMainMenuLeft));
		state.Connect("OnLobbyMenuLeft", Callable.From(OnLobbyMenuLeft));
		state.Connect("OnLobbyLeft", Callable.From(OnLobbyLeft));
		state.Connect("OnGameLeft", Callable.From(OnGameLeft));
		
		state.Transition(GameState.MainMenu);
	}
	
	private void OnMainMenuEntered()
	{
		var mainMenu = GD.Load<PackedScene>("res://Scenes/MainMenu/MainMenu.tscn").Instantiate<MainMenu>();
		mainMenu.Name = "MainMenu";
		
		mainMenu.OnSelectMultiPlayer += () => state.Transition(GameState.LobbyMenu);
		mainMenu.OnSelectSinglePlayer += () => state.Transition(GameState.Game);
		
		menusNode.AddChild(mainMenu);
	}
	
	private void OnLobbyMenuEntered()
	{
		var lobbyMenu = GD.Load<PackedScene>("res://Scenes/LobbySelectionMenu/LobbySelectionMenu.tscn").Instantiate<LobbySelectionMenu>();
		lobbyMenu.Name = "LobbyMenu";

		lobbyMenu.OnSelectBack += () => state.Transition(GameState.MainMenu);
		
		menusNode.AddChild(lobbyMenu);
	}
	
	private void OnLobbyEntered()
	{
		throw new NotImplementedException();
	}

	private void OnGameEntered()
	{
		var game = GD.Load<PackedScene>("res://Scenes/Map1.tscn").Instantiate();
		game.Name = "Game";

		game.GetNode<PauseHandler>("PauseHandler").OnQuitGame += () => state.Transition(GameState.MainMenu);
		
		gameplayNode.AddChild(game);
	}
	
	private void OnMainMenuLeft()
	{
		menusNode.GetNode("MainMenu").QueueFree();
	}
	
	private void OnLobbyMenuLeft()
	{
		menusNode.GetNode("LobbyMenu").QueueFree();
	}
	
	private void OnLobbyLeft()
	{
		throw new NotImplementedException();
	}

	private void OnGameLeft()
	{
		gameplayNode.GetNode("Game").QueueFree();
	}
	
	private enum GameState : uint {
		MainMenu,
		LobbyMenu,
		Lobby,
		Game,
	}
}