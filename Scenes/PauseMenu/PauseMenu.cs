using System.Linq;
using Godot;
using Godot.Collections;
using Steamworks;
using Steamworks.Data;

namespace NightmareNegotiations;

public partial class PauseMenu : Control, IUserInterface
{
	[Export] public VBoxContainer PlayerContainer { get; private set; }
	
	[Signal] public delegate void OnPauseEventHandler();

	[Rpc(CallLocal = true)]
	public void RefreshPlayerList(Array<Variant> players)
	{
		PlayerContainer.GetChildren().ToList().ForEach(node => node.QueueFree());
		foreach (var player in players)
		{
			var playerInfo = GD.Load<PackedScene>("res://Scenes/PauseMenu/PlayerTemplate.tscn")
				.Instantiate<PlayerInfo>();
			playerInfo.Name = player.ToString();
			PlayerContainer.AddChild(playerInfo);
		}
	}
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("game_pause"))
		{
			HandlePause();
		}
	}
	
    public void OnResumeButtonPressed() => HandlePause();
    
    public void OnQuitButtonPressed()
    {
    }

    private void HandlePause()
    {
	    Visible = !Visible;

	    if (Visible)
	    {
		    EmitSignal(SignalName.OnPause);
	    }
    }

	public void Activate()
	{
		Visible = true;

		// TODO: Handle all things that need to happen whilst paused.
	}

	public void Deactivate()
	{
		Visible = false;

		// TODO: Handle all things that need to happen when you need to unpause.
	}
}
