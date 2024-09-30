using Godot;

namespace NightmareNegotiations.net;

public partial class MultiplayerClient : WebRtcClient
{
    private WebRtcMultiplayerPeer rtcMpP = new();
    private bool isSealed;

    public MultiplayerClient()
    {
        PeerConnected += Connected;
        PeerDisconnected += Disconnected;
    }

    private new void Connected(int id)
    {
        if (ShouldMesh)
        {
            rtcMpP.CreateMesh(id);
        } else if (id == 1)
        {
            rtcMpP.CreateServer();
        }
        else
        {
            rtcMpP.CreateClient(id);
        }
        
        GetTree().GetMultiplayer().MultiplayerPeer = rtcMpP;
    }
    
    private new void Disconnected(int id)
    {
        
    }
}