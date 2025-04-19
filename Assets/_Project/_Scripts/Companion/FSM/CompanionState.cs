public abstract class CompanionState
{
    protected CompanionController companion;
    protected CompanionFSM fsm;

    public CompanionState(CompanionController companion, CompanionFSM fsm)
    {
        this.companion = companion;
        this.fsm = fsm;
    }

    public virtual CompanionStateType StateType => CompanionStateType.Unknown;

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void Tick() { }           // Regular update
    public virtual void FixedTick() { }      // Physics update
}
public enum CompanionStateType
{
    Unknown,
    Idle,
    Follow,
    Investigate,
    InteractWithObject,
    Glitch
}