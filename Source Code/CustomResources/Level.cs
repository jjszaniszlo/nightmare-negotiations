using Godot;

namespace NightmareNegotiations.CustomResources;

public partial class Level : Resource
{
    [Export(PropertyHint.Range, "1,4")]
    public int Difficulty { get; private set; }
    
    [Export]
    public int Reward { get; private set; }
}