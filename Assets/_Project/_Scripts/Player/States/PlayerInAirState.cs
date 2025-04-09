using UnityEngine;

public class PlayerInAirState : PlayerBaseState
{
    public PlayerInAirState(StateMachine stateMachine, PlayerController controller) : base(stateMachine, controller) { }

    public override void LogicUpdate()
    {
        Vector2 input = InputManager.Instance.MoveInput;
        controller.Move(input, applyMovement: !controller.IsTouchingFrontWall());

        if (controller.lastGroundedTime > 0 && controller.HasBufferedJump())
        {
            Debug.Log("Coyote jump triggered from InAirState!");
            controller.ClearJumpBuffer();
            stateMachine.SetState(((PlayerStateMachine)stateMachine).JumpState);
            return;
        }

        if (controller.rb.linearVelocity.y < 0)
        {
            stateMachine.SetState(((PlayerStateMachine)stateMachine).FallState);
        }
    }
}
