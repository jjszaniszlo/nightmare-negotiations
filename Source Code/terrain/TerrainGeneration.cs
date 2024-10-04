using Godot;

namespace NightmareNegotiations.Terrain;

public partial class TerrainGeneration : Node3D
{
	[Export]
	private MeshInstance3D terrainMesh;

	[Export] private int sizeDepth = 100;
	[Export] private int sizeWidth = 100;
	[Export] private int meshResolution = 1;
	
	[Export] private FastNoiseLite noise;
	
	[Export] private Material material;
	
	public override void _Ready()
	{
		GenerateTerrain();	
	}

	private void GenerateTerrain()
	{
		var planeMesh = new PlaneMesh();
		planeMesh.Size = new Vector2(sizeWidth, sizeDepth);
		planeMesh.SubdivideDepth = sizeDepth * meshResolution - 1;
		planeMesh.SubdivideWidth = sizeWidth * meshResolution - 1;
		planeMesh.Material = material;
	}

	public override void _Process(double delta)
	{
	}
}