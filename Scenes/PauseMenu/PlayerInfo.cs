using Godot;

public partial class PlayerInfo : Control
{
    public string Name { get; set; } = "Player";

    public override void _Ready()
    {
        GetNode<Label>("Container/PlayerName").Text = Name;
    }
}
