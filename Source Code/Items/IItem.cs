using Godot;

namespace NightmareNegotiations.Items;

public interface IItem
{
    public void Enable(ItemManager itemManager);
    public void Disable(ItemManager itemManager);
    public void OnUse();
}