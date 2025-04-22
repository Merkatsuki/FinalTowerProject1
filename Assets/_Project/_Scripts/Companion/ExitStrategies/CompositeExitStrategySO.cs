using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Strategies/Exit/Composite Exit")]
public class CompositeExitStrategySO : ExitStrategySO
{
    public enum LogicMode { All, Any }

    [SerializeField] private LogicMode mode = LogicMode.All;
    [SerializeField] private List<ExitStrategySO> strategies;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target)
    {
        if (mode == LogicMode.All)
        {
            foreach (var strategy in strategies)
                if (strategy != null && !strategy.ShouldExit(actor, target)) return false;
            return true;
        }
        else // Any
        {
            foreach (var strategy in strategies)
                if (strategy != null && strategy.ShouldExit(actor, target)) return true;
            return false;
        }
    }
}