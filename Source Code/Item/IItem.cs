using Godot;

namespace NightmareNegotiations;

public interface IItem
{
    public void Enable(ItemManager itemManager);
    public void Disable(ItemManager itemManager);
    public void OnUse();
}