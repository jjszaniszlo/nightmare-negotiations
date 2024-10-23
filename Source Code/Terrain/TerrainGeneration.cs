using Godot;

namespace NightmareNegotiations.Terrain;

[Tool]
public partial class TerrainGeneration : StaticBody3D
{
    [Export]
    public bool DoGenerate
    {
        get => true;
        set => GenerateTerrain();
    }
    
    private int seed = 2;
    public int Seed
    {
        get => seed;
        set
        {
            seed = value;
            baseNoise.Seed = seed;
        }
    }

    public float RadialDropOff { get; set; } = 0.2f;
    
    public float BaseAmplitude { get; set; } = 5.0f;
    
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
    
    public float HillAmplitude { get; set; } = 10.0f;
    
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
    
    public float MountainAmplitude { get; set; } = 300.0f;
    
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
    
    private FastNoiseLite baseNoise = new();
    private FastNoiseLite hillNoise = new();
    private FastNoiseLite mountainNoise = new();

    public TerrainGeneration()
    {
        baseNoise.Seed = Seed;
        baseNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth;
        
        hillNoise.Seed = Seed;
        hillNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        
        mountainNoise.Seed = Seed;
        mountainNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        
        baseNoise.Frequency = BaseFrequency;
        hillNoise.Frequency = HillFrequency;
        mountainNoise.Frequency = MountainFrequency;
    }
    
    public void GenerateTerrain()
    {
        GD.Print("generating terrain mesh...");
        var planeMesh = new PlaneMesh();
        planeMesh.Size = new Vector2(1024, 1024);
        planeMesh.SubdivideWidth = 1023;
        planeMesh.SubdivideDepth = 1023;

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
        GetNode<CollisionShape3D>("TerrainShape").Shape = GetNode<MeshInstance3D>("TerrainMesh").Mesh.CreateTrimeshShape();
    }
}