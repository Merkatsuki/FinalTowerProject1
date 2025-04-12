public class CompanionFSM
{
    private CompanionState currentState;

    public void Initialize(CompanionState startingState)
    {
        currentState = startingState;
        currentState.OnEnter();
    }

    public void ChangeState(CompanionState newState)
    {
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }

    public void Tick() => currentState?.Tick();
    public void FixedTick() => currentState?.FixedTick();

    public string GetCurrentStateName()
    {
        return currentState?.GetType().Name;
    }
}
