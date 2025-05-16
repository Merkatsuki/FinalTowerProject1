using Momentum;
using System.Collections.Generic;
using UnityEngine;

public class CompanionFSM
{
    private CompanionStatusUI statusUI;
    private CompanionState currentState;
    private Stack<CompanionState> stateStack = new();

    public CompanionStateType CurrentStateType => currentState?.StateType ?? CompanionStateType.Unknown;

    public void Initialize(CompanionState startingState, CompanionStatusUI statusUI)
    {
        this.statusUI = statusUI;
        currentState = startingState;
        currentState.OnEnter();
        this.statusUI?.UpdateIcon(currentState.StateType);
    }

    public void ChangeState(CompanionState newState)
    {
        Debug.Log($"(CompanionFSM) Transition: {currentState?.StateType} → {newState.StateType}");
        currentState?.OnExit();
        currentState = newState;
        currentState.OnEnter();
        statusUI?.UpdateIcon(currentState.StateType);
    }

    public void PushState(CompanionState newState)
    {
        if (currentState != null)
        {
            stateStack.Push(currentState);
            currentState.OnExit();
        }

        currentState = newState;
        currentState.OnEnter();
        statusUI?.UpdateIcon(currentState.StateType);
    }

    public void PopState()
    {
        if (stateStack.Count > 0)
        {
            currentState.OnExit();
            currentState = stateStack.Pop();
            currentState.OnEnter();
            statusUI?.UpdateIcon(currentState.StateType);
        }
    }

    public void SafePopOrFollow(CompanionController companion)
    {
        if (stateStack.Count > 0)
        {
            PopState();
        }
        else
        {
            ChangeState(companion.followState);
        }
    }

    public void ResumeDefault(CompanionController companion)
    {
        if (CompanionCommandManager.Instance?.IsWaitHereToggled() == true)
        {
            ChangeState(companion.waitHereState);
        }
        else if (InputManager.instance.IsCommandMode)
        {
            ChangeState(companion.idleState);
        }
        else
        {
            ChangeState(companion.followState);
        }
    }

    public void ClearStateStack()
    {
        stateStack.Clear();
    }

    public CompanionState PeekState()
    {
        return stateStack.Count > 0 ? stateStack.Peek() : null;
    }

    public void Tick() => currentState?.Tick();
    public void FixedTick() => currentState?.FixedTick();

    public string GetCurrentStateName()
    {
        return currentState?.GetType().Name;
    }
}