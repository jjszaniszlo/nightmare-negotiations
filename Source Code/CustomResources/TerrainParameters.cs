using Godot;
using Godot.Collections;

namespace NightmareNegotiations.CustomResources;

[Tool]
public partial class TerrainParameters : Resource
{
    [Export] public int Seed { get; private set; }
    [Export] public Array<NoiseParams> TerrainNoises { get; private set; }
}