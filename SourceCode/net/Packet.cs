using Godot;

namespace NightmareNegotiations.net;

public partial class Packet : RefCounted
{
    public MessageType type;
    public int id;
    public string data;
}