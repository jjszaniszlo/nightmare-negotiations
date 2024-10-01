using Godot;

namespace NightmareNegotiations.net;

public partial class MultiplayerState : Node
{
    public static MultiplayerState Instance { get; private set; }

    public bool HostModeEnabled;
    public bool MultiplayerModeEnabled;

    public MultiplayerState()
    {
        Instance = this;
    }
}