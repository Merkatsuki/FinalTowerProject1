using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(StateMachine stateMachine, PlayerController controller) : base(stateMachine, controller) { }

    public override void LogicUpdate()
    {
        if (!controller.IsGrounded())
        {
            stateMachine.SetState(((PlayerStateMachine)stateMachine).InAirState);
            return;
        }

        if (InputManager.Instance.WasJumpPressedThisFrame)
        {
            stateMachine.SetState(((PlayerStateMachine)stateMachine).JumpState);
            return;
        }
    }
}