using Godot;
using System;
using NightmareNegotiations.Networking;

public partial class LobbyScene : Node3D
{
    [Export] public Node3D PlayerSpawnNode { get; private set; }
    public void AddPlayer(long peerId)
    {
        var player = GD.Load<PackedScene>("res://Game Objects/player.tscn").Instantiate<Node3D>();
        player.Name = peerId.ToString();
        player.Position = new Vector3(0.0f, 1.5f, 0.0f);
        
        PlayerSpawnNode.AddChild(player);
    }
    
    public void RemovePlayer(long peerId)
    {
        PlayerSpawnNode.GetNode(peerId.ToString()).QueueFree();
    }
}
