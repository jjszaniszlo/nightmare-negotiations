using Godot;
using NightmareNegotiations.Scenes.Lobby;
using NightmareNegotiations.Scenes.LobbySelectionMenu;
using NightmareNegotiations.Scenes.MainMenu;

namespace NightmareNegotiations;

public partial class GameManager : Node
{
	private LobbyManager lobbyManager = new();
	private Node menusNode;
	private Node gameplayNode;
	private StateMachine state = new();

	public override void _Ready()
	{
		menusNode = GetParent().GetNode<Node>("Menus");
		gameplayNode = GetParent().GetNode<Node>("Gameplay");
		
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
		
		AddChild(lobbyManager);
	}
	
	private void OnMainMenuEntered()
	{
		var mainMenu = GD.Load<PackedScene>("res://Scenes/MainMenu/MainMenu.tscn").Instantiate<MainMenu>();
		mainMenu.Name = "MainMenu";
		
		mainMenu.OnSelectMultiPlayer += () => state.Transition(GameState.LobbyMenu);
		mainMenu.OnSelectSinglePlayer += () => state.Transition(GameState.Lobby);
		
		menusNode.AddChild(mainMenu);
	}
	
	private void OnLobbyMenuEntered()
	{
		var lobbyMenu = GD.Load<PackedScene>("res://Scenes/LobbySelectionMenu/LobbySelectionMenu.tscn")
			.Instantiate<LobbySelectionMenu>();
		lobbyMenu.Name = "LobbyMenu";
		
		lobbyMenu.OnSelectBack += () => state.Transition(GameState.MainMenu);

		lobbyMenu.OnSelectHostLobby += lobbyManager.OnHostLobbyButtonSelected;
		lobbyMenu.OnSelectRefreshLobbyList += lobbyManager.OnRefreshLobbyListButtonSelected;
		lobbyMenu.OnSelectJoinLobby += lobbyManager.OnJoinLobbyButtonSelected;

		lobbyManager.TransitionLobbyScene += () => state.Transition(GameState.Lobby);
		
		menusNode.AddChild(lobbyMenu);
	}

	private void OnLobbyEntered()
	{
		var lobby = GD.Load<PackedScene>("res://Scenes/Lobby/Lobby.tscn").Instantiate<LobbyScene>();
		lobby.Name = "Lobby";
		
		lobby.GetNode<PauseManager>("PauseManager").OnQuitGame += () => state.Transition(GameState.MainMenu);
		lobby.GetNode("ComputerInterfaceManager")
			.GetNode<ComputerInterface>("ComputerInterface").OnLevelSelected += () => 
		{
			GD.Print("Pressed!");
			state.Transition(GameState.Game);
		};

		lobbyManager.AddPlayer += lobby.AddPlayer;
		lobbyManager.RemovePlayer += lobby.RemovePlayer;
		
		gameplayNode.AddChild(lobby);
	}

	private void OnGameEntered()
	{
		var game = GD.Load<PackedScene>("res://Scenes/TerrainGenerationTest.tscn").Instantiate();
		game.Name = "Game";

		game.GetNode<PauseManager>("PauseManager").OnQuitGame += () => state.Transition(GameState.MainMenu);
		
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
		gameplayNode.GetNode("Lobby").QueueFree();
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
