using Godot;

namespace NightmareNegotiations.Interactable;

public partial class ComputerInteractable : Interactable
{
    public override void Interact()
    {
        EmitSignal(Interactable.SignalName.OnInteract);
    }
}