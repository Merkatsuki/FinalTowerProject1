using Momentum.States;
using Momentum;
using UnityEngine;

public class PlayerCommandState : PlayerBaseState
{
    public PlayerCommandState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName)
        : base(player, stateMachine, playerData, playerChecks, playerReferences, animName) { }

    public override void Enter()
    {
        base.Enter();
        _player.SetVelocityZero();
        Debug.Log("[Player] Entered Command Mode State");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // If Command Mode is turned off, go back to idle
        if (!InputManager.instance.IsCommandMode)
        {
            _stateMachine.ChangeState(_player.IdleState);
        }
    }
}