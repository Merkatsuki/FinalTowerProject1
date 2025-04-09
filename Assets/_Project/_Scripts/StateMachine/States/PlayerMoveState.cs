using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(StateMachine stateMachine, PlayerController controller) : base(stateMachine, controller) { }

    public override void Enter()
    {
        base.Enter();
        controller.animator?.SetBool("IsMoving", true);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Vector2 input = InputManager.Instance.MoveInput;
        controller.Move(input);

        if (Mathf.Abs(input.x) < 0.1f)
        {
            stateMachine.SetState(((PlayerStateMachine)stateMachine).IdleState);
        }
    }

    public override void Exit()
    {
        controller.animator?.SetBool("IsMoving", false);
    }
}