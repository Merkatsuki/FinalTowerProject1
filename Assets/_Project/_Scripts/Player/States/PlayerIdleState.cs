using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(StateMachine stateMachine, PlayerController controller) : base(stateMachine, controller) { }

    public override void Enter()
    {
        base.Enter();
        controller.StopMovement();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Vector2 input = InputManager.Instance.MoveInput;

        if (Mathf.Abs(input.x) > 0.1f)
        {
            stateMachine.SetState(((PlayerStateMachine)stateMachine).MoveState);
        }
    }
}