using System;
using Godot;

namespace NightmareNegotiations.Terrain;

// [Tool]
public partial class TerrainGenerator : StaticBody3D
{
    // [Export]
    // public bool DoGenerate
    // {
    //     get => true;
    //     set => GenerateTerrain();
    // }

    [Export] public Mesh GrassInstance;
    
    private int seed = 3;
    public int Seed
    {
        get => seed;
        set
        {
            seed = value;
            baseNoise.Seed = seed;
            hillNoise.Seed = seed;
            mountainNoise.Seed = seed;
        }
    }

    public float RadialDropOff { get; set; } = 0.05f;
    
    public float BaseAmplitude { get; set; } = 2.0f;
    
    private float baseFrequency = 0.01f;
    public float BaseFrequency
    {
        get => baseFrequency;
        set
        {
            baseFrequency = value;
            baseNoise.Frequency = baseFrequency;
        }
    }
    
    public float HillAmplitude { get; set; } = 5.0f;
    
    private float hillFrequency = 0.001f;
    public float HillFrequency
    {
        get => hillFrequency;
        set
        {
            hillFrequency = value;
            hillNoise.Frequency = hillFrequency;
        }
    }
    
    public float MountainAmplitude { get; set; } = 150.0f;
    
    private float mountainFrequency = 0.001f;
    public float MountainFrequency
    {
        get => mountainFrequency;
        set
        {
            mountainFrequency = value;
            mountainNoise.Frequency = mountainFrequency;
        }
    }

    private float treeFrequency = 0.001f;

    public float TreeFrequency
    {
        get => treeFrequency;
        set
        {
            treeFrequency = value;
        }
    }
    
    private FastNoiseLite baseNoise = new();
    private FastNoiseLite hillNoise = new();
    private FastNoiseLite mountainNoise = new();
    private FastNoiseLite treeNoise = new();
    
    public override void _Ready()
    {
        GenerateTerrain();
    }

    public TerrainGenerator()
    {
        baseNoise.Seed = Seed;
        baseNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth;
        baseNoise.Frequency = BaseFrequency;
        
        hillNoise.Seed = Seed;
        hillNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        hillNoise.Frequency = HillFrequency;
        
        mountainNoise.Seed = Seed;
        mountainNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        mountainNoise.Frequency = MountainFrequency;

        treeNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
        treeNoise.Seed = Seed + Random.Shared.Next();
        treeNoise.Frequency = TreeFrequency;
    }
    
    public void GenerateTerrain()
    {
        GD.Print("generating terrain mesh...");
        var planeMesh = new PlaneMesh();
        planeMesh.Size = new Vector2(1024, 1024);
        planeMesh.SubdivideWidth = 127;
        planeMesh.SubdivideDepth = 127;

        var surfaceTool = new SurfaceTool();
        surfaceTool.CreateFrom(planeMesh, 0);
        var data = new MeshDataTool();

        var arrayPlane = surfaceTool.Commit();
        data.CreateFromSurface(arrayPlane, 0);

        for (int i = 0; i < data.GetVertexCount(); i++)
        {
            var v = data.GetVertex(i);
            v.Y = baseNoise.GetNoise2D(v.X, v.Z) * BaseAmplitude 
                  + hillNoise.GetNoise2D(v.X, v.Z) * HillAmplitude 
                  + mountainNoise.GetNoise2D(v.X, v.Z) * MountainAmplitude
                  - Mathf.Sqrt(v.X*v.X + v.Z*v.Z) * RadialDropOff;
            v.Y = Mathf.Clamp(v.Y, -200f, 200.0f);
            data.SetVertex(i, v);
        }
        
        arrayPlane.ClearSurfaces();
        data.CommitToSurface(arrayPlane);
        
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        surfaceTool.CreateFrom(arrayPlane, 0);
        surfaceTool.GenerateNormals();


        GetNode<MeshInstance3D>("TerrainMesh").Mesh = surfaceTool.Commit();
        GetNode<MeshInstance3D>("TerrainMesh").MaterialOverride = GD.Load<Material>("res://Materials/basic_terrain_material.tres");
        GetNode<CollisionShape3D>("TerrainShape").Shape = GetNode<MeshInstance3D>("TerrainMesh").Mesh.CreateTrimeshShape();
        
        GenerateTrees(data);
        AddGrass();
    }

    private readonly PackedScene[] treeScenes =
    {
        GD.Load<PackedScene>("res://World Objects/trees/tree_01.tscn"),
        GD.Load<PackedScene>("res://World Objects/trees/tree_03.tscn"),
    };
    public void GenerateTrees(MeshDataTool meshData)
    {
        GD.Print("Generate trees...");
        var treesNode = GetParent().GetNode<Node3D>("Trees");
        foreach(var child in treesNode.GetChildren())
        {
            child.QueueFree();
        }

        for (int i = 0; i < meshData.GetFaceCount(); i++)
        {
            var vertex = meshData.GetVertex(meshData.GetFaceVertex(i, Random.Shared.Next() % 3));
            var noiseValue = treeNoise.GetNoise2D(vertex.X, vertex.Z);
            if (noiseValue >= -0.3f)
            {
                var tree = treeScenes[Random.Shared.Next() % treeScenes.Length].Instantiate<Node3D>();
                tree.Position = FindRandomVectorWithin3Vectors(
                    meshData.GetVertex(meshData.GetFaceVertex(i, 0)),
                    meshData.GetVertex(meshData.GetFaceVertex(i, 1)),
                    meshData.GetVertex(meshData.GetFaceVertex(i, 2)));

                var radiusScale = (float)Random.Shared.NextDouble() * 0.5f + 1.0f;
                tree.Scale = new Vector3(
                    radiusScale,
                    (float)Random.Shared.NextDouble()*0.5f + 1.0f,
                    radiusScale
                );
                
                tree.RotateY((float)Random.Shared.NextDouble() * Mathf.Pi * 2.0f);
                treesNode.AddChild(tree);
            }
        }
    }

    private void AddGrass()
    {
        var grassMultiMeshInstance = GetNode<MultiMeshInstance3D>("Grass");

        var multiMesh = new MultiMesh();
        multiMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
        multiMesh.InstanceCount = 512;
        multiMesh.Mesh = GrassInstance;

        grassMultiMeshInstance.Multimesh = multiMesh;
    }

    private Vector3 FindRandomVectorWithin3Vectors(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        var randA = v1.Lerp(v2, (float)Random.Shared.NextDouble()); 
        return randA.Lerp(v3, (float)Random.Shared.NextDouble());
    }
}