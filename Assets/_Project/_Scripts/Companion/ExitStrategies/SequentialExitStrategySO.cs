// SequentialExitStrategySO.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Exit Strategy/Sequential")]
public class SequentialExitStrategySO : ExitStrategySO
{
    public List<ExitStrategySO> steps;

    private int currentStepIndex = 0;

    public override void OnEnter(CompanionController companion, InteractableBase target)
    {
        currentStepIndex = 0;
        if (steps.Count > 0)
        {
            steps[0]?.OnEnter(companion, target);
        }
    }

    public override bool ShouldExit(CompanionController companion, InteractableBase target)
    {
        if (steps.Count == 0) return true;

        var current = steps[currentStepIndex];
        if (current.ShouldExit(companion, target))
        {
            currentStepIndex++;

            if (currentStepIndex >= steps.Count)
                return true;

            steps[currentStepIndex].OnEnter(companion, target);
        }

        return false;
    }
}