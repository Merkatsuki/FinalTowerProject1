using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private State currentState;

    public void SetState(State state)
    {
        currentState?.Exit();
        currentState = state;
        currentState?.Enter();
    }

    private void Update()
    {
        currentState?.Tick();
    }

    private void FixedUpdate()
    {
        currentState?.FixedTick();
    }

    private void OnDrawGizmos()
    {
        currentState?.DebugGizmos();
    }
}
