using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Strategies/Exit/Composite Exit")]
public class CompositeExitStrategySO : ExitStrategySO
{
    [SerializeField] private LogicMode logicMode = LogicMode.All;
    [SerializeField] private List<ExitStrategySO> subStrategies;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target)
    {
        if (logicMode == LogicMode.All)
        {
            foreach (var strategy in subStrategies)
                if (strategy != null && !strategy.ShouldExit(actor, target)) return false;
            return true;
        }
        else // Any
        {
            foreach (var strategy in subStrategies)
                if (strategy != null && strategy.ShouldExit(actor, target)) return true;
            return false;
        }
    }

    public void SetLogicMode(LogicMode mode)
    {
        logicMode = mode;
    }

    public void SetSubStrategies(List<ExitStrategySO> strategies)
    {
        subStrategies = strategies;
    }

}