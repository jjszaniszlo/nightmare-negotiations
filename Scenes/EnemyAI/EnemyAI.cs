using Godot;
using NightmareNegotiations.EnemyAIScripts;

namespace NightmareNegotiations.Scenes.EnemyAI;

public partial class EnemyAI : CharacterBody3D
{
    //mainly debugging stage. Does not chase players
    [Export]
    public float PatrolSpeed = 5f; // Units per second

    [Export]
    public float ChaseSpeed = 10f;

    [Export]
    public float AttackRange = 2f; 

    [Export]
    public NodePath PlayerPath;

    [Export]
    public Vector3[] PatrolPoints = new Vector3[] { new Vector3(0, 0, 0), new Vector3(10, 0, 0) };

    private CharacterBody3D player;
    private StateMachine stateMachine;
    
    public StateMachine StateMachine => stateMachine;
    public Vector3 Position => GlobalTransform.Origin;
    public CharacterBody3D Player => player;

    public override void _Ready()
    {
        if (!string.IsNullOrEmpty(PlayerPath))
        {
            player = GetNode<CharacterBody3D>(PlayerPath);
            GD.Print($"{Name} found player at {player.GlobalTransform.Origin}");
        }
        else
        {
            GD.Print($"{Name} has no PlayerPath assigned.");
        }

        if (PatrolPoints == null || PatrolPoints.Length == 0)
        {
            GD.PrintErr($"{Name} has no PatrolPoints assigned.");
        }

        stateMachine = new StateMachine();
        stateMachine.ChangeState(new PatrolState(this, PatrolPoints));

        // group for easier managing 
        AddToGroup("enemies");
    }
    public override void _Process(double delta)
    {
        _PhysicsProcess(delta);

    }

    public override void _PhysicsProcess(double delta)
    {
        stateMachine.Update((float)delta);
    }
    
    public bool IsPlayerInRange()
    {
        if (player == null)
            return false;

        float detectionRadius = 15f;
        bool inRange = GlobalTransform.Origin.DistanceTo(player.GlobalTransform.Origin) <= detectionRadius;
        GD.Print($"{Name} IsPlayerInRange: {inRange}");
        return inRange;
    }
    
    public bool IsPlayerInAttackRange()
    {
        if (player == null)
            return false;

        bool inAttackRange = GlobalTransform.Origin.DistanceTo(player.GlobalTransform.Origin) <= AttackRange;
        GD.Print($"{Name} IsPlayerInAttackRange: {inAttackRange}");
        return inAttackRange;
    }
    
    public void MoveTowards(Vector3 target, float speed, float delta)
    {
        Vector3 direction = (target - GlobalTransform.Origin).Normalized();
        Velocity = direction * speed;
        GD.Print($"{Name} moving towards {target} with velocity {Velocity}");
        MoveAndSlide();
    }
    
    public void AttackPlayer()
    {
        GD.Print($"{Name} is attacking the player!");
        //more code later for health etc. 
    }
    
}
