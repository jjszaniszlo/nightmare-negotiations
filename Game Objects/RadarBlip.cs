using Godot;

public partial class RadarBlip : Node2D
{
    public Node3D Target { get; set; } 
    public void UpdatePosition(Vector3 playerPosition, float radarScale, float radarRadius)
    {
        if (Target == null)
        {
            Visible = false;
            return;
        }
        
        // calculate relative position of the enemy to player
        Vector3 relativePos = Target.GlobalTransform.Origin - playerPosition;
        float distance = relativePos.Length();
        if (distance > radarRadius)
        {
            Visible = false;
            //GD.Print("target out of range");
        }
        else
        {
            Visible = true;
            Vector2 radarPosition = new Vector2(relativePos.X, relativePos.Z) * radarScale;
            Position = radarPosition;
        }
    }
}