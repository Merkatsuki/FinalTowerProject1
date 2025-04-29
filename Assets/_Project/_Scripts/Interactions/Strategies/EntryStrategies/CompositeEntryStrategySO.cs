using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Strategies/Entry/Composite Entry")]
public class CompositeEntryStrategySO : EntryStrategySO
{
    public enum LogicMode { All, Any }

    [SerializeField] private LogicMode logicMode = LogicMode.All;
    [SerializeField] private List<EntryStrategySO> subStrategies;

    public override bool CanEnter(IPuzzleInteractor actor, IWorldInteractable target)
    {
        if (logicMode == LogicMode.All)
        {
            foreach (var strategy in subStrategies)
                if (strategy != null && !strategy.CanEnter(actor, target)) return false;
            return true;
        }
        else // Any
        {
            foreach (var strategy in subStrategies)
                if (strategy != null && strategy.CanEnter(actor, target)) return true;
            return false;
        }
    }

    public void SetCompositeMode(LogicMode mode)
{
    logicMode = mode;
}

public void SetSubStrategies(List<EntryStrategySO> strategies)
{
    subStrategies = strategies;
}
}