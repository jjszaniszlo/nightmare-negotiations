using Godot;

namespace NightmareNegotiations.net;

public interface IServer
{
    public Error Listen(ushort port);
}