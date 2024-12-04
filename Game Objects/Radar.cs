using Godot;
using System.Collections.Generic;

public partial class Radar : CanvasLayer
{
    [Export] public float RadarScale = 3.5f; 
    [Export] public float RadarRadius = 100f; 
    private Node3D _player; 
    private Node2D _blipContainer;
    private ShaderMaterial _radarShaderMaterial;
    private Control RadarBackground;

    public override void _Ready()
    
    {
        
        RadarBackground = GetNode<Control>("RadarBackground");
        _radarShaderMaterial = (ShaderMaterial)RadarBackground.Material;
        SetProcess(true);
        _player = GetNode<Node3D>("..");
        _blipContainer = GetNode<Node2D>("RadarBackground/BlipContainer");
        TrackTreesInGroup();
    }

    public void AddBlip(Node3D target)
    {
        // Load the Blip scene
        PackedScene radarBlipScene = (PackedScene)ResourceLoader.Load("res://Game Objects/RadarBlip.tscn");
        RadarBlip blipInstance = (RadarBlip)radarBlipScene.Instantiate();
        blipInstance.Target = target;
        _blipContainer.AddChild(blipInstance);
    }

    public override void _Process(double delta)
    {
        if (_player == null) return;
        //update blip position relative to player
        foreach (RadarBlip blip in _blipContainer.GetChildren())
        {
            blip.UpdatePosition(_player.GlobalTransform.Origin, RadarScale, RadarRadius);
        }
    }
    private void TrackTreesInGroup()
    {
        foreach (Node3D enemy in GetTree().GetNodesInGroup("trees"))
        {
            AddBlip(enemy);
        }
    }

}

