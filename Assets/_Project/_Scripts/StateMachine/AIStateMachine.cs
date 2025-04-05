using UnityEngine;

public class AIStateMachine : StateMachine<AIState>, IGameStateListener
{
    public float wanderRadius = 2f;
    public float speed = 2f;

    private Vector2 wanderTarget;
    private Animator animator;

    protected override void Initialize()
    {
        animator = GetComponent<Animator>();
        GameStateManager.Instance?.RegisterListener(this);
        ChangeState(AIState.Idle);
    }

    private void OnDestroy()
    {
        GameStateManager.Instance?.UnregisterListener(this);
    }

    private void Update()
    {
        if (!GameStateManager.Instance.Is(GameState.Gameplay)) return;

        switch (CurrentState)
        {
            case AIState.Idle:
                if (Random.value < 0.01f)
                    ChangeState(AIState.Wander);
                break;

            case AIState.Wander:
                transform.position = Vector2.MoveTowards(transform.position, wanderTarget, speed * Time.deltaTime);
                if (Vector2.Distance(transform.position, wanderTarget) < 0.1f)
                    ChangeState(AIState.Idle);
                break;
        }
    }

    protected override void OnStateEnter(AIState state)
    {
        switch (state)
        {
            case AIState.Wander:
                wanderTarget = (Vector2)transform.position + Random.insideUnitCircle * wanderRadius;
                break;
        }

        animator?.SetInteger("AIState", (int)state);
    }

    public void OnGameStateChanged(GameState newState)
    {
        if (newState != GameState.Gameplay)
            ChangeState(AIState.Idle);
    }
}