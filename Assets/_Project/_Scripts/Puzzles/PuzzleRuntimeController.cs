using UnityEngine;
using System.Collections.Generic;

public class PuzzleRuntimeController : MonoBehaviour
{
    [SerializeField] private PuzzleProfileSO puzzleProfile;

    private HashSet<string> completedSteps = new();

    private void Start()
    {
        if (puzzleProfile == null)
        {
            Debug.LogWarning("PuzzleRuntimeController has no profile assigned.");
            return;
        }

        PuzzleManager.Instance.RegisterPuzzle(puzzleProfile);
        PuzzleManager.Instance.RegisterActivePuzzle(puzzleProfile);
    }

    public void HandleInteraction(CompanionClueInteractable clue, RobotInteractionSO interaction, EnergyType energy)
    {
        foreach (var step in puzzleProfile.steps)
        {
            if (completedSteps.Contains(step.stepId)) continue;
            if (step.targetClue != clue) continue;
            if (step.requiredInteraction != null && step.requiredInteraction != interaction) continue;
            if (step.requiredEnergy != EnergyType.None && step.requiredEnergy != energy) continue;

            completedSteps.Add(step.stepId);

            if (!string.IsNullOrEmpty(step.flagToSet))
                PuzzleManager.Instance.SetFlag(step.flagToSet);

            if (step.markStepWhenInteractionTriggered)
                Debug.Log($"Step {step.stepId} completed in puzzle '{puzzleProfile.puzzleId}'");

            CheckPuzzleSolved();
        }
    }

    private void CheckPuzzleSolved()
    {
        if (puzzleProfile.requireAllStepsToSolve)
        {
            foreach (var step in puzzleProfile.steps)
            {
                if (!completedSteps.Contains(step.stepId))
                    return;
            }
        }

        PuzzleManager.Instance.MarkPuzzleSolved(puzzleProfile.puzzleId);
        puzzleProfile.onPuzzleSolved?.Invoke();
        PuzzleManager.Instance.UnregisterActivePuzzle(puzzleProfile);
    }
}