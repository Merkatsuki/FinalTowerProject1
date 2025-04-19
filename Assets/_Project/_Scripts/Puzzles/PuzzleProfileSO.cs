using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Puzzles/Puzzle Profile")]
public class PuzzleProfileSO : ScriptableObject
{
    [Header("Basic Info")]
    public string puzzleId;
    public bool requireAllStepsToSolve = true;

    [Header("Puzzle Steps")]
    public List<PuzzleStep> steps;

    [Header("Solved Event")]
    public UnityEvent onPuzzleSolved;
}
