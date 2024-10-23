using System;
using System.Collections.Generic;
using Godot;

namespace NightmareNegotiations.Utils;

public partial class StateMachine : Node 
{
    private readonly Dictionary<uint, string> states = new();
    public uint CurrentState { get; private set; } = uint.MaxValue;

    [Signal] public delegate void OnTransitionStartEventHandler(uint fromStateId, uint toStateId);
    [Signal] public delegate void OnTransitionEndEventHandler(uint fromStateId, uint toStateId);

    public void AddState(Enum stateIdEnum, string stateName)
    {
        var stateId = Convert.ToUInt32(stateIdEnum);
        AddUserSignal($"On{stateName}Entered");
        AddUserSignal($"On{stateName}Left");
        states[stateId] = stateName;
    }

    public void Transition(Enum toStateIdEnum)
    {
        var toStateId = Convert.ToUInt32(toStateIdEnum);
        if (states.ContainsKey(toStateId))
        {
            // transition from invalid state to valid state.
            if (states.TryGetValue(CurrentState, out var currentState))
            {
                EmitSignal($"On{currentState}Left");
                EmitSignal(SignalName.OnTransitionStart, CurrentState, toStateId);
            }

            CurrentState = toStateId;
            EmitSignal(SignalName.OnTransitionEnd, CurrentState, toStateId);
            EmitSignal($"On{states[CurrentState]}Entered");
        }
        else
        {
            GD.Print($"[State Machine] No such state {toStateId}");
        }
    }

    public async void TransitionDelay(Enum toStateIdEnum, float delay)
    {
        var toStateId = Convert.ToUInt32(toStateIdEnum);
        if (states.ContainsKey(toStateId))
        {
            // transition from invalid state to valid state.
            if (states.TryGetValue(CurrentState, out var currentState))
            {
                EmitSignal($"On{currentState}Left");
                EmitSignal(SignalName.OnTransitionStart, CurrentState, toStateId);
            }

            await ToSignal(GetTree().CreateTimer(delay), Timer.SignalName.Timeout);
            
            CurrentState = toStateId;
            EmitSignal(SignalName.OnTransitionEnd, CurrentState, toStateId);
            EmitSignal($"On{states[CurrentState]}Entered");
        }
        else
        {
            GD.Print($"[State Machine] No such state {toStateId}");
        }

    }
}
