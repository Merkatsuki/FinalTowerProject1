using UnityEngine;

[CreateAssetMenu(menuName = "Companion/ExitStrategies/Exit On Player Command")]
public class ExitOnPlayerCommandSO : ExitStrategySO
{
    public override bool ShouldExit(CompanionController companion, CompanionClueInteractable target)
    {
        // Exit if there's a new or different command pending
        return companion.HasPendingPlayerCommand() && !companion.WasCommanded(target);
    }
}