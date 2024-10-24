using System;
using System.Linq;
using Godot;
using Godot.Collections;

namespace NightmareNegotiations.CustomResources;

[GlobalClass, Tool]
public partial class TerrainNoise : Resource
{
    [ExportCategory("Terrain Settings")]
    private int seed;
    [Export]
    public int Seed
    {
        get => seed;
        set
        {
            foreach (var noise in TerrainNoises) noise.SetSeed(value);
            seed = value;
        }
    }
    [Export] public float RadialDropOff { get; private set; }
    [Export] public float MinHeight;
    [Export] public float MaxHeight;
    [Export] public Array<AmplifiedNoise> TerrainNoises { get; private set; } = new();

    public float GetTerrainNoise2D(float x, float y)
    { 
        var noiseSum = TerrainNoises
            .Select(n => n.GetAmplifiedNoise2D(x, y))
            .Sum();
        
        // sqrt is an expensive operation, only do it if needed.
        var radialOffset = 0.0f;
        if (RadialDropOff >= 0.0f)
        {
            radialOffset = Mathf.Sqrt(x * x + y * y) * RadialDropOff;
        }
        return Mathf.Clamp(noiseSum - radialOffset, MinHeight, MaxHeight);
    }
}