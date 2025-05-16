using UnityEngine;
using System.Collections.Generic;

public class PuzzleController : MonoBehaviour
{
    public enum PuzzleState { NotStarted, InProgress, Solved, Failed }

    [SerializeField] private List<MonoBehaviour> puzzleComponents = new();
    [SerializeField] private bool autoSolveWhenAllReported = true;
    [SerializeField] private List<FlagSO> flagsToSetOnSolve;

    private HashSet<FeatureBase> activatedComponents = new();
    private PuzzleState currentState = PuzzleState.NotStarted;

    private void Awake()
    {
        foreach (var comp in puzzleComponents)
        {
            if (comp is FeatureBase feature)
                feature.RegisterToPuzzle(this);
        }
    }

    public void ReportComponentSuccess(FeatureBase comp)
    {
        if (currentState == PuzzleState.Solved || currentState == PuzzleState.Failed)
            return;

        activatedComponents.Add(comp);
        currentState = PuzzleState.InProgress;

        if (autoSolveWhenAllReported && activatedComponents.Count == puzzleComponents.Count)
            SolvePuzzle();
    }

    public void SolvePuzzle()
    {
        if (currentState == PuzzleState.Solved) return;
        currentState = PuzzleState.Solved;

        Debug.Log("[PuzzleController] Puzzle Solved.");
        QuipManager.Instance.TryPlayFilteredQuip(QuipTriggerType.OnPuzzleSolve, null);
        foreach (var flag in flagsToSetOnSolve)
        {
            if (flag != null)
                FlagManager.Instance?.SetBool(flag, true);
        }
    }

    public void FailPuzzle()
    {
        if (currentState == PuzzleState.Solved) return;
        currentState = PuzzleState.Failed;

        Debug.Log("[PuzzleController] Puzzle Failed.");
        QuipManager.Instance.TryPlayFilteredQuip(QuipTriggerType.OnPuzzleFail, null);
    }

    public void ResetPuzzle()
    {
        currentState = PuzzleState.NotStarted;
        activatedComponents.Clear();

        foreach (var comp in puzzleComponents)
            if (comp is FeatureBase feature)
                feature.ResetPuzzleComponent();
    }

    public PuzzleState GetCurrentState() => currentState;
}

