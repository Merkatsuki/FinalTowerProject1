using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    public State CurrentState { get; private set; }

    protected virtual void Start()
    {
        CurrentState?.Enter();
    }

    protected virtual void Update()
    {
        CurrentState?.LogicUpdate();
    }

    protected virtual void FixedUpdate()
    {
        CurrentState?.PhysicsUpdate();
    }

    public void SetState(State newState)
    {
        if (CurrentState == newState) return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
    }
}