using Godot;

namespace NightmareNegotiations.Scenes.PopUp;

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