using System;
using Godot;

namespace NightmareNegotiations.scenes;

public partial class World : Node3D
{
    private WorldEnvironment environment;
    private DirectionalLight3D moon;
    private DirectionalLight3D sun;

    public override void _Ready()
    {
        environment = GetNode<WorldEnvironment>("WorldEnvironment");
        moon = GetNode<DirectionalLight3D>("Moon");
        sun = GetNode<DirectionalLight3D>("Sun");

        if (IsInstanceValid(sun))
        {
            sun.Position = Vector3.Zero;
            sun.Rotation = Vector3.Zero;
            sun.RotationOrder = EulerOrder.Zxy;
        }
    }

    public void Update()
    {
        
    }
}