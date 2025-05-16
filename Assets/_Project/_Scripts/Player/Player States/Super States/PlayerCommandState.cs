using Momentum.States;
using Momentum;
using UnityEngine;
using System;

public class PlayerCommandState : PlayerBaseState
{
    private IWorldInteractable currentHover;
    private LayerMask _interactableMask;

    public PlayerCommandState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName)
        : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
    {
        _interactableMask = _playerReferences.PInteractor.InteractableMask;
    }

    public override void Enter()
    {
        base.Enter();
        _player.SetVelocityZero();
        Debug.Log("[Player] Entered Command Mode State");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!InputManager.instance.IsCommandMode)
        {
            _stateMachine.ChangeState(_player.IdleState);
            return;
        }

        // Pointer-based interaction target check
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(InputManager.instance.MousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mouseWorld, _interactableMask);

        IWorldInteractable newHover = null;

        if (hit != null && hit.TryGetComponent(out IWorldInteractable candidate))
        {
            if (_playerReferences.PInteractor.Companion.Perception.CanInteractWith(candidate))
            {
                newHover = candidate;
            }
        }

        _playerReferences.PInteractor.UpdateCommandHover();

    }

    public override void Exit()
    {
        base.Exit();
        if (currentHover != null)
        {
            currentHover?.SetHighlight(false);
            currentHover = null;
        }

        ClearCommandHover();
    }

    private void ClearCommandHover()
    {
        if (_playerReferences.PInteractor.commandHoverTarget != null)
        {
            _playerReferences.PInteractor.commandHoverTarget.SetHighlight(false);
            _playerReferences.PInteractor.commandHoverTarget = null;
        }
    }
}
