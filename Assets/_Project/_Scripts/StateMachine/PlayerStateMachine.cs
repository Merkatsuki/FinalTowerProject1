using UnityEngine;

[RequireComponent(typeof(PlayerController2D))]
public class PlayerStateMachine : StateMachine<PlayerState>, IGameStateListener
{
    private PlayerController2D controller;

    public static event System.Action<PlayerState, PlayerState> OnPlayerStateChanged;

    protected override void Initialize()
    {
        controller = GetComponent<PlayerController2D>();
        GameStateManager.Instance?.RegisterListener(this);
        ChangeState(PlayerState.Idle);
    }

    private void OnDestroy()
    {
        GameStateManager.Instance?.UnregisterListener(this);
    }

    private void Update()
    {
        if (!GameStateManager.Instance.Is(GameState.Gameplay))
            return;

        Vector2 moveInput = InputManager.Instance.GetMoveInput();
        bool jumpPressed = InputManager.Instance.GetJumpPressed();

        switch (CurrentState)
        {
            case PlayerState.Idle:
                if (Mathf.Abs(moveInput.x) > 0.1f)
                    ChangeState(PlayerState.Walking);
                if (jumpPressed && controller.IsGrounded())
                    ChangeState(PlayerState.Jumping);
                break;

            case PlayerState.Walking:
                controller.Move(moveInput);
                if (Mathf.Abs(moveInput.x) < 0.1f)
                    ChangeState(PlayerState.Idle);
                if (jumpPressed && controller.IsGrounded())
                    ChangeState(PlayerState.Jumping);
                break;

            case PlayerState.Jumping:
                controller.Jump();
                ChangeState(PlayerState.Idle); // Simplified
                break;

            case PlayerState.Interacting:
            case PlayerState.Frozen:
                controller.StopMovement();
                break;
        }
    }

    public override bool CanEnterState(PlayerState newState)
    {
        switch (newState)
        {
            case PlayerState.Jumping:
                return controller.IsGrounded();
            case PlayerState.Walking:
            case PlayerState.Idle:
                return GameStateManager.Instance.Is(GameState.Gameplay);
            default:
                return true;
        }
    }

    public override void ChangeState(PlayerState newState)
    {
        if (!Equals(CurrentState, newState) && CanEnterState(newState))
        {
            var previousState = CurrentState;
            OnStateExit(CurrentState);
            CurrentState = newState;
            OnStateEnter(CurrentState);
            OnPlayerStateChanged?.Invoke(previousState, CurrentState);
        }
    }

    public void OnGameStateChanged(GameState newState)
    {
        if (newState != GameState.Gameplay)
            ChangeState(PlayerState.Frozen);
    }
}