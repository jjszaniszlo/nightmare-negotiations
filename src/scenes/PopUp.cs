using Godot;

namespace NightmareNegotiations.scenes;

public partial class PopUp : Control
{
    public void SetMessage(string message)
    {
        GetNode<Label>("Panel/VBoxContainer/Label").Text = message;
    }

    private void OnClosePressed()
    {
        QueueFree();
    }
}