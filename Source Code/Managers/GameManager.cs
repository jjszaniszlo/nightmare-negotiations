using System;
using Godot;
using NightmareNegotiations.Scenes.Lobby;
using NightmareNegotiations.Scenes.LobbySelectionMenu;
using NightmareNegotiations.Scenes.MainMenu;

namespace NightmareNegotiations;

public partial class GameManager : Node
{
	[Export] public Node MenusNode { get; private set; }
	[Export] public Node GameplayNode { get; private set; }

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
		mainMenu.OnSelectSinglePlayer += () => state.Transition(GameState.Lobby);
		
		MenusNode.AddChild(mainMenu);
	}
	
	private void OnLobbyMenuEntered()
	{
		var lobbyMenu = GD.Load<PackedScene>("res://Scenes/LobbySelectionMenu/LobbySelectionMenu.tscn").Instantiate<LobbySelectionMenu>();
		lobbyMenu.Name = "LobbyMenu";

		lobbyMenu.OnSelectBack += () => state.Transition(GameState.MainMenu);
		
		MenusNode.AddChild(lobbyMenu);
	}
	
	private void OnLobbyEntered()
	{
		var lobby = GD.Load<PackedScene>("res://Scenes/Lobby/Lobby.tscn").Instantiate();
		lobby.Name = "Lobby";
		
		lobby.GetNode<PauseManager>("PauseManager").OnQuitGame += () => state.Transition(GameState.MainMenu);
		lobby.GetNode("ComputerInterfaceManager")
			.GetNode<ComputerInterface>("ComputerInterface").OnLevelSelected += () => 
		{
			GD.Print("Pressed!");
			state.Transition(GameState.Game);
		};
		
		GameplayNode.AddChild(lobby);
	}

	private void OnGameEntered()
	{
		var game = GD.Load<PackedScene>("res://Scenes/TerrainGenerationTest.tscn").Instantiate();
		game.Name = "Game";

		game.GetNode<PauseManager>("PauseManager").OnQuitGame += () => state.Transition(GameState.MainMenu);
		
		GameplayNode.AddChild(game);
	}
	
	private void OnMainMenuLeft()
	{
		MenusNode.GetNode("MainMenu").QueueFree();
	}
	
	private void OnLobbyMenuLeft()
	{
		MenusNode.GetNode("LobbyMenu").QueueFree();
	}
	
	private void OnLobbyLeft()
	{
		GameplayNode.GetNode("Lobby").QueueFree();
	}

	private void OnGameLeft()
	{
		GameplayNode.GetNode("Game").QueueFree();
	}
	
	private enum GameState : uint {
		MainMenu,
		LobbyMenu,
		Lobby,
		Game,
	}
}