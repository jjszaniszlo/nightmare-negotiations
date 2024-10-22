using System;
using System.Collections.Generic;
using Godot;

namespace NightmareNegotiations.Utils;

public partial class StateMachine : RefCounted
{
    private readonly Dictionary<uint, string> states = new();
    
    // backing field for property
    private uint currentState;
    public uint CurrentState
    {
        get => currentState;
        private set
        {
            EmitSignal(SignalName.OnLeaveCurrentState, currentState);
            currentState = value;
            EmitSignal(SignalName.OnEnterState, currentState);
        }
    }

    [Signal] public delegate void OnTransitionEventHandler();
    [Signal] public delegate void OnEnterStateEventHandler(long newState);
    [Signal] public delegate void OnLeaveCurrentStateEventHandler(long leftState);

    public void AddState(Enum stateIdEnum, string stateName)
    {
        var stateId = Convert.ToUInt32(stateIdEnum);
        states[stateId] = stateName;
    }

    public void Transition(Enum toStateIdEnum)
    {
        var toStateId = Convert.ToUInt32(toStateIdEnum);
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
