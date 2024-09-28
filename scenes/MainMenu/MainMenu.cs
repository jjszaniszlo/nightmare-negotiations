using Godot;

namespace NightmareNegotiations.scenes.MainMenu;

public partial class MainMenu : Node
{
    private PackedScene popUpTemplate = GD.Load<PackedScene>("res://scenes/PopUp/PopUp.tscn");
    private PackedScene worldTemplate = GD.Load<PackedScene>("res://scenes/world.tscn");
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
            GetNode<Control>("UsernameEntryBox").Visible = false;
            GetNode<Control>("Main Buttons").Visible = true;
        }
    }

    private void OnUserNameEntryTextSubmitted(string text)
    {
        // submit
        OnUserNameEntryProceedPressed();
    }

    private void OnSinglePlayerButtonPressed()
    {
        GetParent().AddChild(worldTemplate.Instantiate());
        QueueFree();
    }
}