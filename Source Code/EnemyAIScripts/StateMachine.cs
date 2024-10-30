namespace NightmareNegotiations.EnemyAIScripts;

public class StateMachine
{
	private IState currentState;
	private IState previousState;
	
	// Gets the current active state.

	public IState CurrentState => currentState; //not using

	// Changes the current state to a new state.
	//"newState" The new state to transition into
	
	public void ChangeState(IState newState)
	{
		if (currentState != null)
		{
			currentState.Exit();
			previousState = currentState;
		}

		currentState = newState;
		currentState.Enter();
	}


	// Updates the current state.

	//"delta" Time elapsed since the last frame
	public void Update(float delta)
	{
		currentState?.Execute(delta);
	}


	// Reverts to the previous state.

	public void RevertToPreviousState()
	{
		if (previousState != null)
		{
			ChangeState(previousState);
		}
	}
}