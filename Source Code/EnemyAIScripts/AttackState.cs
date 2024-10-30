using Godot;
namespace NightmareNegotiations.EnemyAIScripts;

public class AttackState : IState
{
	private readonly Scenes.EnemyAI.EnemyAI enemy;

	public AttackState(Scenes.EnemyAI.EnemyAI enemy)
	{
		this.enemy = enemy;
	}

	public void Enter()
	{
		GD.Print($"{enemy.Name} entering Attack State");
	}

	public void Execute(float delta)
	{
		enemy.AttackPlayer();

		// Transition back to Chase if player moves away
		if (!enemy.IsPlayerInAttackRange())
		{
			enemy.StateMachine.ChangeState(new ChaseState(enemy));
		}
	}

	public void Exit()
	{
		GD.Print($"{enemy.Name} exiting Attack State");
	}
}