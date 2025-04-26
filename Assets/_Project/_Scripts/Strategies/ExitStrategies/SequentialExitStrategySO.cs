using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Strategies/Exit/Sequential Exit")]
public class SequentialExitStrategySO : ExitStrategySO
{
    [SerializeField] private List<ExitStrategySO> steps;
    private int currentIndex = 0;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target)
    {
        if (steps == null || steps.Count == 0) return true;

        while (currentIndex < steps.Count)
        {
            var current = steps[currentIndex];
            if (current != null && current.ShouldExit(actor, target))
            {
                currentIndex++;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public override void OnEnter(IPuzzleInteractor actor, IWorldInteractable target)
    {
        currentIndex = 0;
    }
}