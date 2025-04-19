using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Companion/ExitStrategies/Composite")]
public class CompositeExitStrategySO : ExitStrategySO
{
    [SerializeField] private CompositeStrategyMode mode = CompositeStrategyMode.All;
    [SerializeField] private List<ExitStrategySO> strategies;

    public override void OnEnter(CompanionController companion, CompanionClueInteractable target)
    {
        foreach (var strategy in strategies)
            strategy?.OnEnter(companion, target);
    }

    public override bool ShouldExit(CompanionController companion, CompanionClueInteractable target)
    {
        if (strategies == null || strategies.Count == 0)
            return true; // No conditions = default to exit allowed

        switch (mode)
        {
            case CompositeStrategyMode.All:
                foreach (var strategy in strategies)
                {
                    if (strategy != null && !strategy.ShouldExit(companion, target))
                        return false;
                }
                return true;

            case CompositeStrategyMode.Any:
                foreach (var strategy in strategies)
                {
                    if (strategy != null && strategy.ShouldExit(companion, target))
                        return true;
                }
                return false;

            default:
                return true;
        }
    }
}
