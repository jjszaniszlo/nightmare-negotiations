namespace NightmareNegotiations.EnemyAIScripts;

public interface IState
{
	void Enter();
	void Execute(float delta);
	void Exit();
}
