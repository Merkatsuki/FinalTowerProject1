using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public PlayerController controller;

    public PlayerGroundedState GroundedState;
    public PlayerInAirState InAirState;
    public PlayerIdleState IdleState;
    public PlayerMoveState MoveState;
    public PlayerJumpState JumpState;
    public PlayerFallState FallState;
    public PlayerLandState LandState;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        GroundedState = new PlayerGroundedState(this, controller);
        InAirState = new PlayerInAirState(this, controller);
        IdleState = new PlayerIdleState(this, controller);
        MoveState = new PlayerMoveState(this, controller);
        JumpState = new PlayerJumpState(this, controller);
        FallState = new PlayerFallState(this, controller);
        LandState = new PlayerLandState(this, controller);
    }

    protected override void Start()
    {
        base.Start();
        SetState(IdleState);
    }
}