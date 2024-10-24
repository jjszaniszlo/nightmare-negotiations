using Godot;

namespace NightmareNegotiations.CustomResources;

[GodotClassName("NoiseParams")]
public partial class NoiseParams : Resource
{
    [Export] public int Seed { get; set; }
    [Export] public float Frequency { get; set; }
    [Export] public float Amplitude { get; set; }
    [Export] public FastNoiseLite.NoiseTypeEnum NoiseType { get; set; }

    private FastNoiseLite noise;
}