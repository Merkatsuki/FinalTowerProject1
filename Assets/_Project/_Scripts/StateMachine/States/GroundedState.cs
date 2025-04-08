using UnityEngine;

public class GroundedState : State
{
    private enum SubState { Idle, Walking }
    private SubState currentSubState;
    private bool jumpRequested;
    private float coyoteTimer;
    private float coyoteTimeMax = 0.15f;

    public GroundedState(StateMachine sm) : base(sm) { }

    public override void Enter()
    {
        InputManager.OnJumpRequested += RequestJump;
        jumpRequested = false;
        coyoteTimer = coyoteTimeMax;
        currentSubState = SubState.Idle;
        Debug.Log("Entered Grounded State");
    }

    public override void Tick()
    {
        var player = ((PlayerStateMachine)stateMachine).controller;
        Vector2 moveInput = InputManager.Instance.MoveInput;

        // Substate logic
        switch (currentSubState)
        {
            case SubState.Idle:
                if (Mathf.Abs(moveInput.x) > 0.1f)
                    currentSubState = SubState.Walking;
                break;
            case SubState.Walking:
                player.Move(moveInput);
                if (Mathf.Abs(moveInput.x) < 0.1f)
                {
                    currentSubState = SubState.Idle;
                    player.StopMovement();
                }
                break;
        }

        // Coyote time management
        if (!player.IsGrounded())
            coyoteTimer -= Time.deltaTime;
        else
            coyoteTimer = coyoteTimeMax;

        if (jumpRequested && coyoteTimer > 0)
        {
            jumpRequested = false;
            stateMachine.SetState(new JumpState(stateMachine));
            return;
        }

        if (coyoteTimer <= 0f && !player.IsGrounded())
        {
            stateMachine.SetState(new InAirState(stateMachine));
        }
    }

    public override void Exit()
    {
        InputManager.OnJumpRequested -= RequestJump;
        Debug.Log("Exiting Grounded State");
    }

    public override void RequestJump()
    {
        jumpRequested = true;
    }
}
