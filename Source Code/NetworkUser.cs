using Godot;

namespace NightmareNegotiations.Networking;

public partial class NetworkUser : Node
{
    [Export] public Node3D Root { get; private set; }
    public override void _EnterTree()
    {
        // the name of the network user node is the user id in the p2p setup.
        SetMultiplayerAuthority((int)long.Parse(Root.Name));
    }
}