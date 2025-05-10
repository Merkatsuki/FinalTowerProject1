using UnityEngine;
using System.Collections.Generic;

public class PuzzleController : MonoBehaviour
{
    public enum PuzzleState { NotStarted, InProgress, Solved, Failed }

    [SerializeField] private List<MonoBehaviour> puzzleComponents = new();
    [SerializeField] private bool autoSolveWhenAllReported = true;

    private HashSet<IPuzzleComponent> activatedComponents = new();
    private PuzzleState currentState = PuzzleState.NotStarted;

    private void Awake()
    {
        foreach (var comp in puzzleComponents)
        {
            if (comp is IPuzzleComponent pComp)
                pComp.RegisterToPuzzle(this);
        }
    }

    public void ReportComponentSuccess(IPuzzleComponent comp)
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
        QuipManager.Instance.TryPlayQuip(QuipTriggerType.OnPuzzleSolve);
        // Trigger any puzzle-complete effects, flags, etc.
    }

    public void FailPuzzle()
    {
        if (currentState == PuzzleState.Solved) return;
        currentState = PuzzleState.Failed;

        Debug.Log("[PuzzleController] Puzzle Failed.");
        QuipManager.Instance.TryPlayQuip(QuipTriggerType.OnPuzzleFail);
    }

    public void ResetPuzzle()
    {
        currentState = PuzzleState.NotStarted;
        activatedComponents.Clear();

        foreach (var comp in puzzleComponents)
            if (comp is IPuzzleComponent pComp)
                pComp.ResetPuzzleComponent();
    }

    public PuzzleState GetCurrentState() => currentState;
}

