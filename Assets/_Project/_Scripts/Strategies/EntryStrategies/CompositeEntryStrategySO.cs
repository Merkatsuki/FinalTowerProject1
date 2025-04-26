using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Strategies/Entry/Composite Entry")]
public class CompositeEntryStrategySO : EntryStrategySO
{
    public enum LogicMode { All, Any }

    [SerializeField] private LogicMode mode = LogicMode.All;
    [SerializeField] private List<EntryStrategySO> strategies;

    public override bool CanEnter(IPuzzleInteractor actor, IWorldInteractable target)
    {
        if (mode == LogicMode.All)
        {
            foreach (var strategy in strategies)
                if (strategy != null && !strategy.CanEnter(actor, target)) return false;
            return true;
        }
        else // Any
        {
            foreach (var strategy in strategies)
                if (strategy != null && strategy.CanEnter(actor, target)) return true;
            return false;
        }
    }
}