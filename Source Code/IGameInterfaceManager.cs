using Godot;

namespace NightmareNegotiations;

public interface IGameInterfaceManager
{
    [Signal]
    public delegate void OnInterfaceEnabledEventHandler();
    void EnableRequired();
    void DisableRequired();
}