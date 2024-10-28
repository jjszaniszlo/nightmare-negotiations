using Godot;

namespace NightmareNegotiations;

public partial class ComputerInteractable : Interactable
{
    public override void Interact()
    {
        EmitSignal(Interactable.SignalName.OnInteract);
    }
}