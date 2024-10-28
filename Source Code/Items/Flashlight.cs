using Godot;

namespace NightmareNegotiations.Items;

public partial class Flashlight : Node3D, IItem
{
	[Export] public SpotLight3D Light { get; private set; }
	public override void _Process(double delta)
	{
	}

	public override void _Ready()
	{
		Light.LightEnergy = 0.0f;
	}
	
	public void Enable(Managers.ItemManager itemManager)
	{
		SetProcess(true);
		itemManager.ItemUsed += OnUse;
	}

	public void Disable(Managers.ItemManager itemManager)
	{
		SetProcess(false);
		itemManager.ItemUsed -= OnUse;
	}

	public void OnUse()
	{
		Light.LightEnergy = Light.LightEnergy == 0.0f ? 4.0f : 0.0f;
	}
}