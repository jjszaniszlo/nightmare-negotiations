using System;
using Godot;

namespace NightmareNegotiations;

public partial class ComputerInterface : Control
{
	[Export] public GridContainer GridContainer { get; private set; }

	[Signal]
	public delegate void OnLevelSelectedEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TempCreateLevelElements();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void TempCreateLevelElements()
	{
		var levelButtonTemplate = GetNode<Button>("LevelSelectionTemplate");
		for (int i = 0; i < 15; i++)
		{
			var newLevelButton = (Button)levelButtonTemplate.Duplicate();
			var difficulty = Random.Shared.Next() % 4 + 1;
			var reward = (int)(20*Math.Exp(difficulty*0.5f) + 4.0f*Math.Exp(Random.Shared.NextSingle() * 2.0f));

			for (int j = 0; j < difficulty; j++)
			{
				newLevelButton.GetNode("Difficulty").GetNode<TextureRect>($"Star{j}").Visible = true;
			}

			newLevelButton.Pressed += () =>
			{
				EmitSignal(SignalName.OnLevelSelected);
			};

			newLevelButton.GetNode<RichTextLabel>("Reward").Text =
				$"[font=res://Assets/fonts/space/SpaceCrusaders-x3DP0.ttf][font_size=30][center]${reward}[/center][/font_size][/font]";

			newLevelButton.Visible = true;
			GridContainer.AddChild(newLevelButton);
		}
	}

	public void CreateContractInterfaceElements(Level[] levels)
	{
		
	}
}