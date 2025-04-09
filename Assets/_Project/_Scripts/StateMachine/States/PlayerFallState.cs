using UnityEngine;

public class PlayerFallState : PlayerInAirState
{
    public PlayerFallState(StateMachine stateMachine, PlayerController controller) : base(stateMachine, controller) { }

    public override void Enter()
    {
        base.Enter();
        controller.animator?.SetTrigger("Fall");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (controller.IsGrounded())
        {
            if (controller.HasBufferedJump())
            {
                Debug.Log("Buffered jump triggered from FallState!");
                controller.ClearJumpBuffer();
                stateMachine.SetState(((PlayerStateMachine)stateMachine).JumpState);
            }
            else
            {
                stateMachine.SetState(((PlayerStateMachine)stateMachine).LandState);
            }
        }
    }
}