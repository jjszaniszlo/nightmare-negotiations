using Godot;

namespace NightmareNegotiations.EnemyAIScripts;

public class PatrolState : IState
{
	private readonly Scenes.EnemyAI.EnemyAI enemy;
	private readonly Vector3[] patrolPoints;
	private int currentPatrolIndex; //0 by default

	public PatrolState(Scenes.EnemyAI.EnemyAI enemy, Vector3[] patrolPoints)
	{
		this.enemy = enemy;
		this.patrolPoints = patrolPoints;
	}

	public void Enter()
	{
		GD.Print($"{enemy.Name} entering Patrol State");
	}

	public void Execute(float delta)
	{
		Vector3 target = patrolPoints[currentPatrolIndex];
		enemy.MoveTowards(target, enemy.PatrolSpeed, delta);

		if (enemy.Position.DistanceTo(target) < 1f)
		{
			currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
		}

		// Transition to Chase if player is in range
		if (enemy.IsPlayerInRange())
		{
			enemy.StateMachine.ChangeState(new ChaseState(enemy));
		}
	}

	public void Exit()
	{
		GD.Print($"{enemy.Name} exiting Patrol State");
	}
}