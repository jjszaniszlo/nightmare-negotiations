using Godot;

namespace NightmareNegotiations.net.webrtc;

public partial class MultiplayerRtcClient : WebRtcClient, IClient
{
    private WebRtcMultiplayerPeer rtcMpP = new();
}