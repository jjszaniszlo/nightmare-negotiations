using Godot;

namespace NightmareNegotiations.Terrain;

[Tool]
public partial class TerrainGeneration : StaticBody3D
{
    public void GenerateMesh()
    {
        var planeMesh = new PlaneMesh();
        planeMesh.Size = new Vector2(10, 10);
        planeMesh.SubdivideWidth = 9;
        planeMesh.SubdivideDepth = 9;
        
        
    }
}