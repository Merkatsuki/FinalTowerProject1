using UnityEngine;

public class InAirState : State
{
    private float fallMultiplier = 2.5f;
    private float lowJumpMultiplier = 2f;
    private float gravity = Physics2D.gravity.y;

    public InAirState(StateMachine sm) : base(sm) { }

    public override void Enter()
    {
        Debug.Log("Entered InAir State");
        InputManager.OnJumpRequested += HandleBufferedJump;
    }

    public override void Exit()
    {
        InputManager.OnJumpRequested -= HandleBufferedJump;
        Debug.Log("Exiting InAir State");
    }

    public override void Tick()
    {
        var player = ((PlayerStateMachine)stateMachine).controller;

        // Prevent movement & animation updates midair
        player.Move(Vector2.zero, applyMovement: false);

        float yVel = player.rb.linearVelocity.y;

        bool isJumping = yVel > 0.1f;
        bool isFalling = yVel < -0.1f;

        if (player.animator != null)
        {
            player.animator.SetBool("IsJumping", isJumping);
            player.animator.SetBool("IsFalling", isFalling);
            player.animator.SetFloat("Speed", 0f); // Prevent walk animation

            //Debug.Log($"[AIR] yVel: {yVel:F2} | IsJumping: {isJumping} | IsFalling: {isFalling}");
        }

        if (yVel > 0 && !InputManager.Instance.IsJumpPressed)
        {
            player.rb.linearVelocity += Vector2.up * gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        else if (yVel < 0)
        {
            player.rb.linearVelocity += Vector2.up * gravity * (fallMultiplier - 1) * Time.deltaTime;
        }

        if (player.IsGrounded())
        {
            Debug.Log("[AIR] Landed: Switching to GroundedState");
            player.animator?.SetTrigger("Land");
            player.ResetMomentumPreservation();
            stateMachine.SetState(new GroundedState(stateMachine));
        }
    }

    private void HandleBufferedJump()
    {
        var player = ((PlayerStateMachine)stateMachine).controller;

        if (player.IsTouchingFrontWall() || player.IsTouchingBackWall())
        {
            Debug.Log("[AIR] Wall Jump Triggered");
            player.WallJump();
        }
        else
        {
            Debug.Log("[AIR] Buffered Jump Stored");
            player.BufferJump();
        }
    }
}