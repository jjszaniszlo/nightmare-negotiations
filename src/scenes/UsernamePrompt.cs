using Godot;
using NightmareNegotiations.net;

namespace NightmareNegotiations.scenes;

public partial class UsernamePrompt : Control
{
    private PackedScene popUpTemplate = GD.Load<PackedScene>("res://Scenes/PopUp/PopUp.tscn");
    private PackedScene mainMenuTemplate = GD.Load<PackedScene>("res://Scenes/MainMenu/MainMenu.tscn");
    
    private async void OnUserNameEntryProceedPressed()
    {
        var usernameText = GetNode<LineEdit>("UsernameEntryBox/Panel/Enter Username").Text;
        
        // TODO: Serverside database which keeps track of available usernames.

        if (usernameText.Length == 0 || usernameText.Contains(' '))
        {
            var popUp = popUpTemplate.Instantiate<PopUp>();
            popUp.SetMessage("You need to enter a valid username!");
            AddChild(popUp);
        }
        else
        {
            Main.Globals.User.Client = new Client();
            GetParent().AddChild(Main.Globals.User.Client);
            
            await ToSignal(GetTree().CreateTimer(2.0),Timer.SignalName.Timeout);

            if (Main.Globals.User.Client.IsConnectionOpen())
            {
                Main.Globals.User.Client.SendUserName(usernameText);
                GD.Print("[Log] Sending username to server...");
            }
            else
            {
                GD.Print("[Error] Client not open!");
            }
            
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