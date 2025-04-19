using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Companion/EntryStrategies/Composite")]
public class CompositeEntryStrategySO : EntryStrategySO
{
    [SerializeField] private CompositeStrategyMode mode = CompositeStrategyMode.All;
    [SerializeField] private List<EntryStrategySO> strategies;

    public override bool CanEnter(CompanionController companion, CompanionClueInteractable target)
    {
        if (strategies == null || strategies.Count == 0)
            return true; // No entry conditions = allow by default

        switch (mode)
        {
            case CompositeStrategyMode.All:
                foreach (var strategy in strategies)
                {
                    if (strategy != null && !strategy.CanEnter(companion, target))
                        return false;
                }
                return true;

            case CompositeStrategyMode.Any:
                foreach (var strategy in strategies)
                {
                    if (strategy != null && strategy.CanEnter(companion, target))
                        return true;
                }
                return false;

            default:
                return true;
        }
    }
}