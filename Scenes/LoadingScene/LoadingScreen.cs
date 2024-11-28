using Godot;
using System;

public partial class LoadingScreen : Control
{
    public string LoadScene { get; set; }
    public override void _Ready()
    {
        ResourceLoader.LoadThreadedRequest(LoadScene);
    }

    public override void _Process(double delta)
    {
        var status = ResourceLoader.LoadThreadedGetStatus(LoadScene);
        if (status == ResourceLoader.ThreadLoadStatus.Loaded)
        {
            var newScene = ResourceLoader.LoadThreadedGet(LoadScene);
            GetTree().ChangeSceneToPacked((PackedScene)newScene);
        }
    }
}
