using UnityEngine;
using System.Collections.Generic;

public abstract class RobotInteractionSO : ScriptableObject
{
    [SerializeField] private List<EntryStrategySO> entryStrategies;
    [SerializeField] private List<ExitStrategySO> exitStrategies;
    public bool IsRepeatable = true;

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


    public bool CanExecute(CompanionController companion, CompanionClueInteractable target)
    {
        if (entryStrategies == null || entryStrategies.Count == 0)
            return true;

        foreach (var strategy in entryStrategies)
        {
            if (strategy == null || !strategy.CanEnter(companion, target))
                return false;
        }

        return true;
    }
}
