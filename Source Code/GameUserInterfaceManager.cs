using System.Linq;
using Godot;

namespace NightmareNegotiations;

[GlobalClass]
public partial class GameUserInterfaceManager : Node
{
    [Export] public CanvasLayer PostProcessing { get; private set; }
	[Export] public Node3D PlayerInstances;

    private int numEnabledInterfaces;
    private int oldEnabledInterfaces;
    
    public override void _Process(double delta)
    {
        numEnabledInterfaces = GetChildren()
            .Count(n => ((Control)n).Visible);

        if (numEnabledInterfaces == oldEnabledInterfaces) return;

        if (numEnabledInterfaces > 0)
        {
            PostProcessing.Visible = false;
		    var player = (Node3D)PlayerInstances
			    .GetChildren()
			    .FirstOrDefault(n => long.Parse(((Node3D)n).Name) == NetworkUser.Instance.PeerId);

		    if (player != null)
		    {
			    player.SetPhysicsProcess(false);
			    player.GetNode("Head").SetProcessInput(false);
			    Input.SetMouseMode(Input.MouseModeEnum.Visible);
		    }
        }
        else
        {
            PostProcessing.Visible = true;
		    var player = (Node3D)PlayerInstances
			    .GetChildren()
			    .FirstOrDefault(n => long.Parse(((Node3D)n).Name) == NetworkUser.Instance.PeerId);

		    if (player != null)
		    {
			    player.SetPhysicsProcess(true);
			    player.GetNode("Head").SetProcessInput(true);
			    Input.SetMouseMode(Input.MouseModeEnum.Captured);
		    }
        }

        oldEnabledInterfaces = numEnabledInterfaces;
    }
}