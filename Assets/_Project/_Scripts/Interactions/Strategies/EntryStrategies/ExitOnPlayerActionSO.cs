using UnityEngine;

public enum PlayerActionExitTrigger
{
    OnCompanionInteracts,
    OnPlayerCommandedMove
}

[CreateAssetMenu(menuName = "Strategies/Exit/On Player Action")]
public class ExitOnPlayerActionSO : ExitStrategySO
{
    [SerializeField] private PlayerActionExitTrigger triggerType;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target)
    {
        if (actor is not CompanionController companion) return false;

        return triggerType switch
        {
            PlayerActionExitTrigger.OnCompanionInteracts => companion.GetCurrentTrackedTarget() == null,
            PlayerActionExitTrigger.OnPlayerCommandedMove => !companion.HasPendingPlayerCommand(),
            _ => false
        };
    }
}

