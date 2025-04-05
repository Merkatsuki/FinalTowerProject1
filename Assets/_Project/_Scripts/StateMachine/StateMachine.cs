using UnityEngine;

public abstract class StateMachine<T> : MonoBehaviour where T : System.Enum
{
    public T CurrentState { get; protected set; }

    protected virtual void Start()
    {
        Initialize();
    }

    protected abstract void Initialize();

    public virtual void ChangeState(T newState)
    {
        if (!Equals(CurrentState, newState))
        {
            OnStateExit(CurrentState);
            CurrentState = newState;
            OnStateEnter(CurrentState);
        }
    }

    protected virtual void OnStateEnter(T state) { }
    protected virtual void OnStateExit(T state) { }
}
