using UnityEngine;
using System.Collections.Generic;

public class PuzzleRuntimeController : MonoBehaviour
{
    [SerializeField] private PuzzleProfileSO profile;

    private HashSet<string> completedStepIds = new();
    private bool puzzleSolved = false;

    private void Awake()
    {
        PuzzleManager.Instance.RegisterRuntime(profile.puzzleId, this);
    }

    public void TryProcessInteraction(PuzzleObject target, IPuzzleInteractor actor)
    {
        if (puzzleSolved) return;

        foreach (var step in profile.steps)
        {
            if (completedStepIds.Contains(step.stepId)) continue;
            if (step.targetObject != target) continue;
            if (step.requiredEnergy != EnergyType.None && actor.GetEnergyType() != step.requiredEnergy) continue;

            Debug.Log($"[Puzzle] Step matched: {step.stepId}");

            if (step.markStepWhenInteractionTriggered)
            {
                completedStepIds.Add(step.stepId);
                if (!string.IsNullOrEmpty(step.flagToSet))
                {
                    PuzzleManager.Instance.SetFlag(step.flagToSet);
                }

                CheckIfPuzzleComplete();
                return;
            }
        }
    }

    private void CheckIfPuzzleComplete()
    {
        if (!profile.requireAllStepsToSolve)
        {
            puzzleSolved = true;
            profile.onPuzzleSolved?.Invoke();
            return;
        }

        foreach (var step in profile.steps)
        {
            if (!completedStepIds.Contains(step.stepId)) return;
        }

        puzzleSolved = true;
        Debug.Log($"[Puzzle] Puzzle complete: {profile.puzzleId}");
        profile.onPuzzleSolved?.Invoke();
    }
}