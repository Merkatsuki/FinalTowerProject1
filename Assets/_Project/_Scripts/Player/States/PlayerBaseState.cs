public abstract class PlayerBaseState : State
{
    protected PlayerController controller;
    protected PlayerAnimator Anim => ((PlayerStateMachine)stateMachine).playerAnimator;

    protected PlayerBaseState(StateMachine stateMachine, PlayerController controller) : base(stateMachine)
    {
        this.controller = controller;
    }
}