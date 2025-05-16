using Momentum;
using UnityEngine;

public enum PlayerActionExitTrigger
{
    OnCompanionInteracts,
    OnPlayerCommandedMove,
    OnPlayerMoves,
    OnPlayerJumps,
    OnPlayerUsesUI
}

[CreateAssetMenu(menuName = "Strategies/Exit/On Player Action")]
public class ExitOnPlayerActionSO : ExitStrategySO
{
    [SerializeField] private PlayerActionExitTrigger triggerType;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target)
    {
        if (InputManager.instance == null) return false;

        switch (triggerType)
        {
            case PlayerActionExitTrigger.OnCompanionInteracts:
                if (actor is CompanionController companion)
                    return companion.GetCurrentTrackedTarget() == null;
                break;

            case PlayerActionExitTrigger.OnPlayerCommandedMove:
                if (actor is CompanionController companionCommand)
                    return !companionCommand.HasPendingPlayerCommand();
                break;

            case PlayerActionExitTrigger.OnPlayerMoves:
                return InputManager.instance.WASDInput.magnitude > 0.1f;

            case PlayerActionExitTrigger.OnPlayerJumps:
                return InputManager.instance.JumpPressedThisFrame;

            case PlayerActionExitTrigger.OnPlayerUsesUI:
                return InputManager.instance.IsDialogueMode;

            default:
                return false;
        }

        return false;
    }
}
