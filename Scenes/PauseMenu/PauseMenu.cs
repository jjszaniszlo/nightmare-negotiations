using Godot;

namespace NightmareNegotiations.Scenes.PauseMenu;

public partial class PauseMenu : Control
{
    [Signal]
    public delegate void OnSelectResumeEventHandler();
    
    [Signal]
    public delegate void OnSelectQuitEventHandler();
    
    public void OnResumeButtonPressed()
    {
        EmitSignal(SignalName.OnSelectResume);
    }
    
    public void OnQuitButtonPressed()
    {
        EmitSignal(SignalName.OnSelectQuit);
    }
}