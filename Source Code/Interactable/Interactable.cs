using Godot;

namespace NightmareNegotiations;

public abstract partial class Interactable : Node
{
    [Signal] public delegate void OnInteractEventHandler();
    public abstract void Interact();
}