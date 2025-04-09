using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(StateMachine stateMachine, PlayerController controller) : base(stateMachine, controller) { }

    public override void Enter()
    {
        base.Enter();
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
}