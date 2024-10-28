using Godot;

namespace NightmareNegotiations.Managers.InGameUI;

public interface IGameInterfaceManager
{
    [Signal]
    public delegate void OnInterfaceEnabledEventHandler();
    void EnableRequired();
    void DisableRequired();
}