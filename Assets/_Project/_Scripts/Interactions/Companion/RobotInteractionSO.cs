// RobotInteractionSO.cs
using UnityEngine;
using System.Collections.Generic;

public abstract class RobotInteractionSO : ScriptableObject
{
    [Tooltip("Conditions that determine when this interaction is complete")]
    public List<ExitStrategySO> exitStrategies;

    public abstract void Execute(CompanionController companion, CompanionClueInteractable target);

    public virtual void OnEnter(CompanionController companion, CompanionClueInteractable target)
    {
        foreach (var strategy in exitStrategies)
        {
            strategy?.OnEnter(companion, target);
        }
    }

    public virtual bool ShouldExit(CompanionController companion, CompanionClueInteractable target)
    {
        foreach (var strategy in exitStrategies)
        {
            if (strategy != null && !strategy.ShouldExit(companion, target))
                return false;
        }
        return true;
    }
}
