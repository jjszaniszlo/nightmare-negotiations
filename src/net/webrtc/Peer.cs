using System.Text.Json;
using Godot;

namespace NightmareNegotiations.net.webrtc;

public partial class Peer : RefCounted
{
    public long Id { get; private set; }
    public string LobbyCode { get; private set; }
    public WebSocketPeer WebSocketPeer { get; private set; } = new();

    public Peer(long peerId, StreamPeer tcp)
    {
        Id = peerId;
        WebSocketPeer.AcceptStream(tcp);
    }
    
    public bool IsWebsocketOpen()
    {
        return WebSocketPeer.GetReadyState() == WebSocketPeer.State.Open;
    }

    public Error SendMessage(MessageType type, long id, string data = "")
    {
        return WebSocketPeer.SendText(JsonSerializer.Serialize(new Message
        {
            Type = type,
            Id = id,
            Data = data
        }));
    }
}