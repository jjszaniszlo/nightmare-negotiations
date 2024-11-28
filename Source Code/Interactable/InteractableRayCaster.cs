using Godot;

namespace NightmareNegotiations;

[GlobalClass]
public partial class InteractableRayCaster : RayCast3D
{
    public override void _Process(double delta)
    {
        var collider = GetCollider();

        if (IsColliding() && collider is Interactable interactable)
        {
            if (Input.IsActionJustPressed("interact"))
            {
                interactable.Interact();
            }
        }
    }
}