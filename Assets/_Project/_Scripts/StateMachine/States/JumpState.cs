public class JumpState : State
{
    public JumpState(StateMachine sm) : base(sm) { }

    public override void Enter()
    {
        var player = ((PlayerStateMachine)stateMachine).controller;
        player.Jump();
        stateMachine.SetState(new InAirState(stateMachine));
    }
}