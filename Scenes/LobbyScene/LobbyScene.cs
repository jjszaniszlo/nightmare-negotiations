using Godot;
using System;
using System.Linq;
using Godot.Collections;
using NightmareNegotiations.Scenes.PauseMenu;
using Steamworks.Data;

namespace NightmareNegotiations;

public partial class LobbyScene : Node3D
{
    [Export] public Node3D PlayerInstances { get; private set; }
    [Export] public PauseMenu PauseMenu { get; private set; }

    public override void _Ready()
    {
        Multiplayer.PeerConnected += AddPlayer;

        PauseMenu.OnPause += OnPause;
    }

    private void OnPause()
    {
        var lobby = new Lobby(NetworkUser.Instance.LobbyId);
        var names = lobby.Members.Select(member =>
        {
            return (Variant)member.Name;
        }).ToList();
        
        PauseMenu.RefreshPlayerList(new Array<Variant>(names));
    }

    public void AddPlayer(long peerId)
    {
        var player = GD.Load<PackedScene>("res://Game Objects/player.tscn").Instantiate<PlayerMovement>();
        player.Name = peerId.ToString();
        PlayerInstances.AddChild(player);
        
        GD.Print($"{peerId} connected!");
    }
}
