public abstract class State
{
    protected readonly StateMachine stateMachine;

    protected State(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Tick() { }
    public virtual void FixedTick() { }
    public virtual void Exit() { }
    public virtual void DebugGizmos() { }
    public virtual void RequestJump() { }
}
