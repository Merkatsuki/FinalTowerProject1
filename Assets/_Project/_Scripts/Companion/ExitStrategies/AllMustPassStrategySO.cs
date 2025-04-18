// ExitStrategySO.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Exit Strategy/All Must Pass")]
public class AllMustPassStrategySO : ExitStrategySO
{
    public List<ExitStrategySO> strategies;

    public override void OnEnter(CompanionController companion, CompanionClueInteractable target)
    {
        foreach (var strategy in strategies)
        {
            strategy?.OnEnter(companion, target);
        }
    }

    public override bool ShouldExit(CompanionController companion, CompanionClueInteractable target)
    {
        foreach (var strategy in strategies)
        {
            if (strategy != null && !strategy.ShouldExit(companion, target))
                return false;
        }
        return true;
    }
}
