using Godot;

namespace NightmareNegotiations;

public partial class ComputerInteractable : Interactable
{
    [Export] public ComputerInterface ComputerInterface { get; private set; }
    public override void Interact() => ComputerInterface.Visible = !ComputerInterface.Visible;
}