using Godot;

namespace NightmareNegotiations.net;

public partial class NetworkManager : Node
{
    public static NetworkManager Instance { get; private set; }

    private PackedScene webRtcClientScene;
    private PackedScene webRtcServerScene;
    
    public NetworkType ActiveNetworkType { get; private set; } = NetworkType.WebRTC;
    
    private IClient activeClient;
    private IServer activeServer;
    
    public NetworkManager()
    {
        Instance = this;

        webRtcClientScene = GD.Load<PackedScene>("res://Scenes/Net/WebRtcClient.tscn");
        webRtcServerScene = GD.Load<PackedScene>("res://Scenes/Net/WebRtcServer.tscn");
    }

    public void InitDedicatedServer()
    {
        switch (ActiveNetworkType)
        {
            case NetworkType.WebRTC:
                SetActiveServer(webRtcServerScene);
                GD.Print("[Log] Initialized WebRTC Server!");
                break;
        }
    }
    
    public void InitClient()
    {
        switch (ActiveNetworkType)
        {
            case NetworkType.WebRTC:
                SetActiveClient(webRtcClientScene);
                GD.Print("[Log] Initialized WebRTC Client!");
                break;
        }

        activeClient.Connect("ws://127.0.0.1:45567");
    }
    
    private void SetActiveClient(PackedScene networkScene)
    {
        activeClient = networkScene.Instantiate<IClient>();
        AddChild((Node)activeClient);
    }
    
    private void SetActiveServer(PackedScene networkScene)
    {
        activeServer = networkScene.Instantiate<IServer>();
        AddChild((Node)activeServer);
    }

    public enum NetworkType
    {
        WebRTC,
    }
}