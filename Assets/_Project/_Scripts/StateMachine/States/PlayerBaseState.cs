public abstract class PlayerBaseState : State
{
    protected PlayerController controller;

    protected PlayerBaseState(StateMachine stateMachine, PlayerController controller) : base(stateMachine)
    {
        this.controller = controller;
    }
}