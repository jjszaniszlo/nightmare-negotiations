using System;
using Godot;
using NightmareNegotiations.CustomResources;

namespace NightmareNegotiations.Terrain;

public partial class TreeInstancer : Node3D
{
    [Export] public Node3D FollowTarget { get; set; }
    [Export] public TerrainNoise TerrainNoise { get; set; }
    [Export] public int InstanceCount { get; set; }
    [Export] public int InstanceSpacing { get; set; }
    [Export] public float InstancePositionRandomize { get; set; }
    [Export] public Mesh InstanceMesh { get; set; }
    [Export] public float UpdateFrequency { get; set; }

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
        var followXZ = FollowTarget.GlobalPosition;
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

            pos.Y = TerrainNoise.GetTerrainNoise2D(pos.X, pos.Z);

            var transform = new Transform3D();
            transform.Origin = pos;
            
            multiMesh.SetInstanceTransform(i, transform);
            GD.Print($"instance {i}, {pos}");
        }

        return multiMesh;
    }

    // Randomness that is deterministic.
    private float PsuedoRandom(float x1, float x2)
    {
        return Mathf.PosMod(Mathf.Sin((float)(new Vector2(x1, x2).Dot(new Vector2(12.9898f, 78.233f)) * 43758.5453123)), 1.0f);
    }
    
    // private readonly PackedScene[] treeScenes =
    // {
    //     GD.Load<PackedScene>("res://World Objects/trees/tree_01.tscn"),
    //     GD.Load<PackedScene>("res://World Objects/trees/tree_03.tscn"),
    //     GD.Load<PackedScene>("res://World Objects/trees/tree_29.tscn"),
    //     GD.Load<PackedScene>("res://World Objects/trees/tree_30.tscn"),
    // };
    // public void GenerateTrees(MeshDataTool meshData)
    // {
    //     GD.Print("Generate trees...");
    //     var treesNode = GetParent().GetNode<Node3D>("Trees");
    //     foreach(var child in treesNode.GetChildren())
    //     {
    //         child.QueueFree();
    //     }
    //
    //     for (int i = 0; i < meshData.GetFaceCount(); i++)
    //     {
    //         var vertex = meshData.GetVertex(meshData.GetFaceVertex(i, Random.Shared.Next() % 3));
    //         var noiseValue = treeNoise.GetNoise2D(vertex.X, vertex.Z);
    //         if (noiseValue >= -0.3f)
    //         {
    //             var tree = treeScenes[Random.Shared.Next() % treeScenes.Length].Instantiate<Node3D>();
    //             tree.Position = FindRandomVectorWithin3Vectors(
    //                 meshData.GetVertex(meshData.GetFaceVertex(i, 0)),
    //                 meshData.GetVertex(meshData.GetFaceVertex(i, 1)),
    //                 meshData.GetVertex(meshData.GetFaceVertex(i, 2)));
    //
    //             var radiusScale = (float)Random.Shared.NextDouble() * 0.5f + 1.0f;
    //             tree.Scale = new Vector3(
    //                 radiusScale,
    //                 (float)Random.Shared.NextDouble()*0.5f + 1.0f,
    //                 radiusScale
    //             );
    //             
    //             tree.RotateY((float)Random.Shared.NextDouble() * Mathf.Pi * 2.0f);
    //             treesNode.AddChild(tree);
    //         }
    //     }
    // }
    //
    // private Vector3 FindRandomVectorWithin3Vectors(Vector3 v1, Vector3 v2, Vector3 v3)
    // {
    //     var randA = v1.Lerp(v2, (float)Random.Shared.NextDouble()); 
    //     return randA.Lerp(v3, (float)Random.Shared.NextDouble());
    // }
}