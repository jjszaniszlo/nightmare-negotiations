using Godot;
using System;

namespace NightmareNegotiations;

public partial class LobbyScene : Node3D
{
    [Export] public Node3D PlayerInstances { get; private set; }
    public override void _Ready()
    {
        Multiplayer.PeerConnected += AddPlayer;
    }

    public void AddPlayer(long peerId)
    {
        var player = GD.Load<PackedScene>("res://Game Objects/player.tscn").Instantiate<PlayerMovement>();
        player.Name = peerId.ToString();
        PlayerInstances.AddChild(player);
        
        GD.Print($"{peerId} connected!");
    }
}
