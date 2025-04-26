using Momentum;
using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Exit/ExitAfterPlayerCommandedMove")]
public class ExitAfterPlayerCommandedMoveSO : ExitStrategySO
{
    private bool playerMoved = false;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable interactable)
    {
        return playerMoved;
    }

    public override void OnEnter(IPuzzleInteractor actor, IWorldInteractable interactable)
    {
        playerMoved = false;
        InputManager.instance.OnMoveCommand += HandleMove;
    }

    public override void OnExit(IPuzzleInteractor actor, IWorldInteractable interactable)
    {
        InputManager.instance.OnMoveCommand -= HandleMove;
    }

    private void HandleMove()
    {
        playerMoved = true;
    }
}