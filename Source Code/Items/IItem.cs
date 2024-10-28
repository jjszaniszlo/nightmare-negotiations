using Godot;

namespace NightmareNegotiations.Items;

public interface IItem
{
    public void Enable(Managers.ItemManager itemManager);
    public void Disable(Managers.ItemManager itemManager);
    public void OnUse();
}