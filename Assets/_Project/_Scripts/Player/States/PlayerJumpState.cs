using UnityEngine;

public class PlayerJumpState : PlayerGroundedState
{
    public PlayerJumpState(StateMachine stateMachine, PlayerController controller) : base(stateMachine, controller) { }

    public override void Enter()
    {
        base.Enter();

        controller.StopMovement();

        controller.Jump();

        stateMachine.SetState(((PlayerStateMachine)stateMachine).InAirState);
    }
}