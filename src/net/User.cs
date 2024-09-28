using Godot;
using Godot.Collections;

namespace NightmareNegotiations.net;

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

            // connect SessionDescriptionCreated signal to OnSignalCreated, but pass in connection, so we can
            // do extra stuff in it.
            // this essentially delegates setting the description until it's created by the connection.
            connection.SessionDescriptionCreated += (type, sdp) =>
                OnSignalSessionCreated(type, sdp, connection);

            connection.IceCandidateCreated += (media, index, name) =>
                OnSignalIceCandidateCreated(media, index, name, connection);
            
            connections.Add(peerId, connection);
            rtcPeer.AddPeer(connection, peerId);
        }

        rtcPeer.PeerConnected += OnSignalPeerConnected;
        rtcPeer.PeerDisconnected += OnSignalPeerDisconnected;

        GetTree().GetMultiplayer().MultiplayerPeer = rtcPeer;
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