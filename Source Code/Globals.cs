using System.Collections.Generic;
using Godot;

namespace NightmareNegotiations;

public partial class Globals : Node
{
    public static Globals Instance { get; private set; }

    public Dictionary<string, PackedScene> ItemManifest { get; private set; } = new()
    {
        ["flashlight"] = GD.Load<PackedScene>("res://Game Objects/items/flashlight.tscn"),
    };
    
    public override void _Ready()
    {
        Instance = this;
    }
}