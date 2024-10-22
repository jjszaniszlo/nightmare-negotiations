using Godot;

namespace NightmareNegotiations.Scenes.MainMenu;

public partial class MainMenu : Node
{
    [Signal]
    public delegate void OnSelectSinglePlayerEventHandler();
    [Signal]
    public delegate void OnSelectMultiPlayerEventHandler();
    
    private void OnSinglePlayerButtonPressed()
    {
        EmitSignal(SignalName.OnSelectSinglePlayer);
    }
    
    private void OnMultiPlayerButtonPressed()
    {
        EmitSignal(SignalName.OnSelectMultiPlayer);
    }
}