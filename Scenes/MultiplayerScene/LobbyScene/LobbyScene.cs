using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Steamworks.Data;

namespace NightmareNegotiations;

public partial class LobbyScene : Node3D
{
    public override void _Ready()
    {
        Multiplayer.PeerConnected += AddPlayer;

		var levelList = GenerateLevels();
		GD.Print($"{levelList}");

        // PauseMenu.OnPause += OnPause;
    }

    private void OnPause()
    {
        var lobby = new Lobby(NetworkUser.Instance.LobbyId);
        var names = lobby.Members.Select(member =>
        {
            return (Variant)member.Name;
        }).ToList();

        // PauseMenu.RefreshPlayerList(new Array<Variant>(names));
    }

    public void AddPlayer(long peerId)
    {
        var player = GD.Load<PackedScene>("res://Game Objects/player.tscn").Instantiate<PlayerMovement>();
        player.Name = peerId.ToString();
        // PlayerInstances.AddChild(player);
        
        GD.Print($"{peerId} connected!");
    }

	// TODO: needs number of levels to generate, for now fix it at 5.
	// TODO: Generate list of levels based on global difficulty.
	private List<Level> GenerateLevels()
	{
		var levelList = new List<Level>();

		for (int i = 0; i < 5; i++)
		{
			var level = new Level();

			var difficulty = Random.Shared.Next() % 4 + 1;

			// TODO: fix magic numbers.
			var reward = (int)
				(20*Math.Exp(difficulty*0.5f)
				+ 4.0f*Math.Exp(Random.Shared.NextSingle() * 2.0f));

			level.Difficulty = difficulty;
			level.Reward = reward;

			levelList.Add(level);
		}

		return levelList;
	}
}
