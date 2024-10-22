using System.Collections.Generic;
using Godot;

namespace NightmareNegotiations.Utils;

public partial class StateMachine : RefCounted
{
    private readonly Dictionary<long, string> states = new();
    
    // backing field for property
    private long currentState;
    public long CurrentState
    {
        get => currentState;
        private set
        {
            EmitSignal(SignalName.OnLeaveCurrentState, currentState);
            currentState = value;
            EmitSignal(SignalName.OnNewState, currentState);
        }
    }

    [Signal] public delegate void OnTransitionEventHandler();
    [Signal] public delegate void OnNewStateEventHandler(long newState);
    [Signal] public delegate void OnLeaveCurrentStateEventHandler(long leftState);

    public void AddState(long stateId, string stateName)
    {
        states[stateId] = stateName;
    }

    public void Transition(long toStateId)
    {
        if (states.ContainsKey(toStateId))
        {
            EmitSignal(SignalName.OnTransition);
            CurrentState = toStateId;
        }
        else
        {
            GD.Print($"[State Machine] No such state {toStateId}");
        }
    }
}
