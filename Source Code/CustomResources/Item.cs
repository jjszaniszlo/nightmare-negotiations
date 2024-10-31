using Godot;

namespace NightmareNegotiations;

public partial class Item : Resource
{
    [Export] public string Name { get; set; }
    [Export] public Mesh Mesh { get; set; }
}