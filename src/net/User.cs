using Godot;
using Godot.Collections;

namespace NightmareNegotiations.net;

// This acts as a global state for a WebRTC user.  It also manages the client.

public partial class User : Node
{
    private Client client;
    private string userName;
    private string hostUserName;
    private string currentLobbyCode;

    private bool isHost;
    private int id;
    private System.Collections.Generic.Dictionary<int, string> peers;

    private System.Collections.Generic.Dictionary<int, WebRtcPeerConnection> connections;
    private WebRtcMultiplayerPeer rtcPeer;

    private void InitConnection()
    {
        rtcPeer = new WebRtcMultiplayerPeer();
        rtcPeer.CreateMesh(id);

        connections.Clear();

        foreach (var peerId in peers.Keys)
        {
            var connection = new WebRtcPeerConnection();
            
            // initialize connection to google's stun servers
            var error = connection.Initialize(new Dictionary
            {
                {
                    "iceServers",
                    new Array { new Dictionary { { "urls", new Array { "stun:stun.l.google.com:19302" } } } }
                }
            });

            if (error != Error.Ok)
            {
                GD.PrintErr(error);
            }

            // connect SessionDescriptionCreated signal to OnSignalCreated, but pass in connection as well.
            // this essentially delegates setting the description until it's created by the connection.
            connection.SessionDescriptionCreated += (type, sdp) =>
                OnSignalSessionCreated(type, sdp, connection);

            // connect IceCandidateCreate signal to OnSignalCandidateCreated, passing in connection, similar to before.
            connection.IceCandidateCreated += (media, index, name) =>
                OnSignalIceCandidateCreated(media, index, name, connection);
            
            // add the connection to both the list of connections and the RtcPeer itself.
            connections.Add(peerId, connection);
            rtcPeer.AddPeer(connection, peerId);
        }

        // callbacks for peer connection changes
        rtcPeer.PeerConnected += OnSignalPeerConnected;
        rtcPeer.PeerDisconnected += OnSignalPeerDisconnected;

        // Enable the godot high level multiplayer api, but utilizing WebRTC.
        GetTree().GetMultiplayer().MultiplayerPeer = rtcPeer;
    }

    public void CreateClient()
    {
        client = new Client();
    }
    
    private void OnSignalSessionCreated(string type, string sdp, WebRtcPeerConnection connection)
    {
        connection.SetLocalDescription(type, sdp);
    }
    
    private void OnSignalIceCandidateCreated(string media, long index, string name, WebRtcPeerConnection connection)
    {
    }

    private void OnSignalPeerConnected(long peerId)
    {
        
    }
    
    private void OnSignalPeerDisconnected(long peerId)
    {
        
    }
}