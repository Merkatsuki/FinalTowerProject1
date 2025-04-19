using UnityEngine;

public class CompanionFSM
{
    private CompanionStatusUI statusUI;
    private CompanionState currentState;
    public CompanionStateType CurrentStateType => currentState?.StateType ?? CompanionStateType.Unknown;

    public void Initialize(CompanionState startingState, CompanionStatusUI statusUI)
    {
        this.statusUI = statusUI;
        currentState = startingState;
        currentState.OnEnter();
        this.statusUI?.UpdateIcon(currentState.StateType); // Set icon immediately
    }

    public void ChangeState(CompanionState newState)
    {
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter();
        statusUI.UpdateIcon(currentState.StateType);
    }

    public void Tick() => currentState?.Tick();
    public void FixedTick() => currentState?.FixedTick();

    public string GetCurrentStateName()
    {
        return currentState?.GetType().Name;

    }


}

