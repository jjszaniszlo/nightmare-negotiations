using System;
using Godot;
using NightmareNegotiations.CustomResources;

namespace NightmareNegotiations.Terrain;

public partial class MeshInstancer : Node3D
{
    [Export] public Node3D FollowTarget { get; set; }
    [Export] public TerrainNoise TerrainNoise { get; set; }
    [Export] public int InstanceCount { get; set; } = 1000;
    [Export] public float InstanceSpacing { get; set; } = 0.5f;
    [Export] public float GridScale { get; set; } = 1.0f;
    [Export] public float InstancePositionRandomize { get; set; } = 0.3f;
    [Export] public float InstanceHeightOffset { get; set; } = -1.0f;
    [Export] public float InstanceScaleRandomize { get; set; } = 0.1f;
    [Export] public float InstanceWidth { get; set; } = 1.0f;
    [Export] public float InstanceLength { get; set; } = 1.0f;
    [Export] public float InstanceHeight { get; set; } = 1.0f;
    [Export] public float InstanceRotationRandomize { get; set; } = 0.1f;
    [Export(PropertyHint.Range, "0,50")] public float InstanceMinimumScale = 1.0f;
    [Export(PropertyHint.Range, "0,PI")] public float InstanceXRotation;
    [Export(PropertyHint.Range, "0,PI")] public float InstanceYRotation;
    [Export(PropertyHint.Range, "0,PI")] public float InstanceZRotation;
    [Export] public Mesh InstanceMesh { get; set; }
    [Export] public float UpdateFrequency { get; set; } = 10.0f;

    private MultiMeshInstance3D multiMeshInstance3D;
    private MultiMesh multiMesh;

    private float instanceRows;
    private float offset;

    private Timer timer;

    public override void _Ready()
    {
        GD.Print("create tree multi mesh!");
        CreateMultiMesh();
    }

    private void CreateMultiMesh()
    {
        multiMeshInstance3D = new MultiMeshInstance3D();
        multiMeshInstance3D.TopLevel = true;

        multiMesh = new MultiMesh();
        multiMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
        multiMesh.InstanceCount = InstanceCount;
        multiMesh.Mesh = InstanceMesh;

        instanceRows = Mathf.Sqrt(InstanceCount);
        offset = Mathf.Round(InstanceCount / instanceRows);
        
        AddChild(multiMeshInstance3D);
        
        timer = new Timer();
        GetParent().CallDeferred(MethodName.AddChild, timer);
        timer.Timeout += UpdateInstancer;
        timer.WaitTime = UpdateFrequency;
        timer.Autostart = true;
        timer.OneShot = false;
        
        UpdateInstancer();
    }

    private void UpdateInstancer()
    {
        var followXZ = FollowTarget.GlobalPosition * new Vector3(1.0f, 0.0f, 1.0f);
        GlobalPosition = followXZ.Snapped(new Vector3(1.0f, 0.0f, 1.0f));

        multiMeshInstance3D.Multimesh = DistributeMeshes();
        
        timer.WaitTime = UpdateFrequency;
    }

    private MultiMesh DistributeMeshes()
    {
        for (int i = 0; i < InstanceCount; i++)
        {
            var pos = GlobalPosition;
            pos.Z = i;
            pos.X = (int)(pos.Z % instanceRows);
            pos.Z = (int)((pos.Z - pos.X) / instanceRows);

            pos.X -= offset * 0.5f;
            pos.Z -= offset * 0.5f;

            pos *= InstanceSpacing;
            pos.X += (int)GlobalPosition.X - (int)GlobalPosition.X % InstanceSpacing;
            pos.Z += (int)GlobalPosition.Z - (int)GlobalPosition.Z % InstanceSpacing;

            pos.X += PsuedoRandom(pos.X, pos.Z) * InstancePositionRandomize;
            pos.Z += PsuedoRandom(pos.X, pos.Z) * InstancePositionRandomize;
            pos.X -= PsuedoRandom(pos.X, pos.Z) * InstancePositionRandomize;
            pos.Z -= PsuedoRandom(pos.X, pos.Z) * InstancePositionRandomize;
            
            var x = pos.X;
            var z = pos.Z;
            var y = TerrainNoise.GetTerrainNoise2D(pos.X, pos.Z) + InstanceHeightOffset;
            var origin = new Vector3(x, y, z);

            var scale = new Vector3(
                InstanceMinimumScale + (PsuedoRandom(x, z) * InstanceScaleRandomize) + InstanceLength,
                InstanceMinimumScale + (PsuedoRandom(x, z) * InstanceScaleRandomize) + InstanceHeight,
                InstanceMinimumScale + (PsuedoRandom(x, z) * InstanceScaleRandomize) + InstanceWidth
            );

            var rot = new Vector3();
            rot.X += InstanceXRotation + PsuedoRandom(x, z) * InstanceRotationRandomize;
            rot.Y += InstanceYRotation + PsuedoRandom(x, z) * InstanceRotationRandomize;
            rot.Z += InstanceZRotation + PsuedoRandom(x, z) * InstanceRotationRandomize;

            var transform = Transform3D.Identity;
            transform.Origin = origin;

            transform = transform.RotatedLocal(transform.Basis.X.Normalized(), rot.X);
            transform = transform.RotatedLocal(transform.Basis.Y.Normalized(), rot.Y);
            transform = transform.RotatedLocal(transform.Basis.Z.Normalized(), rot.Z);
            
            multiMesh.SetInstanceTransform(i, transform.ScaledLocal(scale));
        }

        return multiMesh;
    }

    // Randomness that is deterministic.
    private float PsuedoRandom(float x1, float x2)
    {
        return Mathf.PosMod(Mathf.Sin((float)(new Vector2(x1, x2).Dot(new Vector2(12.9898f, 78.233f)) * 43758.5453123)), 1.0f);
    }
}