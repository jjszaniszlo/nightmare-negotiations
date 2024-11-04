using Godot;

namespace NightmareNegotiations;

public partial class LevelData : Resource
{
    [Export(PropertyHint.Range, "1,4")]
    public int Difficulty { get; set; }
    
    [Export]
    public int Reward { get; set; }
}
