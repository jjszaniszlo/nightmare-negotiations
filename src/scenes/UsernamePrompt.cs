using Godot;

namespace NightmareNegotiations.scenes.UsernamePrompt;

public partial class UsernamePrompt : Control
{
    private PackedScene popUpTemplate = GD.Load<PackedScene>("res://scenes/PopUp/PopUp.tscn");
    private PackedScene mainMenuTemplate = GD.Load<PackedScene>("res://scenes/MainMenu/MainMenu.tscn");
    
    private void OnUserNameEntryProceedPressed()
    {
        var usernameText = GetNode<LineEdit>("UsernameEntryBox/Panel/Enter Username").Text;
        
        // TODO: Serverside database which keeps track of available usernames.

        if (usernameText.Length == 0 || usernameText.Contains(' '))
        {
            var popUp = popUpTemplate.Instantiate<PopUp.PopUp>();
            popUp.SetMessage("You need to enter a valid username!");
            AddChild(popUp);
        }
        else
        {
            GetParent().AddChild(mainMenuTemplate.Instantiate());
            QueueFree();
        }
    }

    private void OnUserNameEntryTextSubmitted(string text)
    {
        // submit
        OnUserNameEntryProceedPressed();
    }
}