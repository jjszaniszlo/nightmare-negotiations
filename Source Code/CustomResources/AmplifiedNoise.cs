using Godot;

namespace NightmareNegotiations.CustomResources;

[GlobalClass, Tool]
public partial class AmplifiedNoise : FastNoiseLite 
{
    [Export] public float Amplitude { get; set; }

    public float GetAmplifiedNoise2D(float x, float y)
    {
        return GetNoise2D(x, y) * Amplitude;
    }
}