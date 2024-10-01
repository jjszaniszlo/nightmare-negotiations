using Godot;

namespace NightmareNegotiations.net.webrtc;

public partial class WebRtcServer : Node
{
    public const ushort Port = 45567;
    
    private TcpServer _tcpServer;

    public WebRtcServer()
    {
        _tcpServer = new TcpServer();

        Listen(Port);
        GD.Print($"[Log] Listening on port {Port}...");
    }

    public override void _Process(double delta)
    {
        Poll();
        Clean();
    }

    public Error Listen(ushort port)
    {
        return _tcpServer.Listen(port);
    }
    
    private void Poll()
    {
        
    }

    private void Clean()
    {
        
    }

    private bool ParseMessage()
    {
        return true;
    }
}