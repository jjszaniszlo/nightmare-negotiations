using Godot;
using Godot.Collections;
using NightmareNegotiations.Items;

namespace NightmareNegotiations;

public partial class ItemManager : Node3D
{
	[Export] public Node3D ItemPivot { get; private set; }
	public int MaxItems { get; private set; } = 5;
	
	[Signal] public delegate void ItemUsedEventHandler();

	public override void _Ready()
	{
		var flashlight = Globals.Instance.ItemManifest["flashlight"].Instantiate<Flashlight>();
		flashlight.Enable(this);
		ItemPivot.AddChild(flashlight);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("itemUse"))
		{
			GD.Print("Item used!");
			EmitSignal(SignalName.ItemUsed);
		}
	}

	public override void _Input(InputEvent @event)
	{
		// if (@event is InputEventAction action)
		// {
		// 	if (action.Action == "itemUse" && action.Pressed)
		// 	{
		// 		GD.Print("Item used!");
		// 		EmitSignal(SignalName.ItemUsed);
		// 	}
		// }
	}
}