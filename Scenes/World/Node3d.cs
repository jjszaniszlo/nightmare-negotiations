using Godot;

namespace NightmareNegotiations.Scenes.World;

public partial class Node3d : Node3D
{
    // Constants for mouse button masks
    private static int mouseButtonMaskLeft = 1 << (mouseButtonMaskLeft - 1);
    private static int MouseButtonMaskRight = 1 << (MouseButtonMaskRight - 1);
    private static int MouseButtonMaskMiddle = 1 << (MouseButtonMaskMiddle - 1);
    
    // References to nodes
    private Camera3D _camera;
   // private Character _robot;

    private float _camRotation = 0.0f;

    public override void _Ready()
    {
        // Initialize node references
        _camera = GetNode<Camera3D>("CameraBase/Camera3D");
       // _robot = GetNode<null>("");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
        {
            // Get closest point on navmesh for the current mouse cursor position.
            Vector2 mouseCursorPosition = mouseEvent.Position;
            float cameraRayLength = 1000.0f;
            Vector3 cameraRayStart = _camera.ProjectRayOrigin(mouseCursorPosition);
            Vector3 cameraRayEnd = cameraRayStart + _camera.ProjectRayNormal(mouseCursorPosition) * cameraRayLength;

            Vector3 closestPointOnNavmesh = NavigationServer3D.MapGetClosestPointToSegment(
                GetWorld3D().NavigationMap,
                cameraRayStart,
                cameraRayEnd
            );
            //_robot.SetTargetPosition(closestPointOnNavmesh);
        }
        else if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            //int buttonMask = mouseMotionEvent.ButtonMask;
            int combinedMask = MouseButtonMaskMiddle | MouseButtonMaskRight;

           // if ((buttonMask & combinedMask) != 0)
            {
              //  _camRotation += mouseMotionEvent.Relative.x * 0.005f;
                GetNode<Node3D>("CameraBase").Rotation = new Vector3(0, _camRotation, 0);
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
	
	
	
}