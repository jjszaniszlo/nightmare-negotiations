using Godot;

namespace NightmareNegotiations.EnemyAIScripts;

public class ChaseState : IState
{
	private readonly Scenes.EnemyAI.EnemyAI enemy;

	public ChaseState(Scenes.EnemyAI.EnemyAI enemy)
	{
		this.enemy = enemy;
	}

	public void Enter()
	{
		GD.Print($"{enemy.Name} entering Chase State");
	}

	public void Execute(float delta)
	{
		Vector3 playerPos = enemy.Player.GlobalTransform.Origin;
		enemy.MoveTowards(playerPos, enemy.ChaseSpeed, delta);

		// Transition back to Patrol if player is out of range
		if (!enemy.IsPlayerInRange())
		{
			enemy.StateMachine.ChangeState(new PatrolState(enemy, enemy.PatrolPoints));
		}

		// Transition to Attack if close enough
		if (enemy.IsPlayerInAttackRange())
		{
			enemy.StateMachine.ChangeState(new AttackState(enemy));
		}
	}

	public void Exit()
	{
		GD.Print($"{enemy.Name} exiting Chase State");
	}
}