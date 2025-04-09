using UnityEngine;

public class PlayerLandState : PlayerGroundedState
{
    public PlayerLandState(StateMachine stateMachine, PlayerController controller) : base(stateMachine, controller) {}

    public override void Enter()
    {
        base.Enter();
        Anim.PlayLand();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Vector2 input = InputManager.Instance.MoveInput;

        if (Mathf.Abs(input.x) > 0.1f)
            stateMachine.SetState(((PlayerStateMachine)stateMachine).MoveState);
        else
            stateMachine.SetState(((PlayerStateMachine)stateMachine).IdleState);
    }

}