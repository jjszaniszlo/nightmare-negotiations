using System;
using Godot;
using NightmareNegotiations.CustomResources;

namespace NightmareNegotiations.Terrain;

public partial class TerrainGenerator : StaticBody3D
{
    [Export] public TerrainNoise TerrainNoise { get; private set; }
    [Export] public int Size { get; set; } = 512;
    [Export] public float DivisionRatio { get; set; } = 0.125f;
    
    public override void _Ready()
    {
        GenerateTerrain();
    }
    
    public void GenerateTerrain()
    {
        GD.Print("generating terrain mesh...");
        var planeMesh = new PlaneMesh();
        planeMesh.Size = new Vector2(Size, Size);
        planeMesh.SubdivideWidth = (int)(Size*DivisionRatio)-1;
        planeMesh.SubdivideDepth = (int)(Size*DivisionRatio)-1;

        var surfaceTool = new SurfaceTool();
        surfaceTool.CreateFrom(planeMesh, 0);
        var data = new MeshDataTool();

        var arrayPlane = surfaceTool.Commit();
        data.CreateFromSurface(arrayPlane, 0);

        for (int i = 0; i < data.GetVertexCount(); i++)
        {
            var v = data.GetVertex(i);
            v.Y = TerrainNoise.GetTerrainNoise2D(v.X, v.Z);
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

        GetViewport().DebugDraw = Viewport.DebugDrawEnum.Wireframe;
    }
}