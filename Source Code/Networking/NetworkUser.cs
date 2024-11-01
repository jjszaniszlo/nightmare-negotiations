using Godot;

namespace NightmareNegotiations;

public partial class NetworkUser : Node
{
    public static NetworkUser Instance { get; private set; }
    public int PeerId { get; set; }
    public bool InLobby { get; set; }
    
    public override void _Ready()
    {
        Instance = this;
    }
}