// ExitStrategySO.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Exit Strategy/On Puzzle Solved")]
public class ExitOnPuzzleSolvedSO : ExitStrategySO
{
    public string puzzleKey;

    public override bool ShouldExit(CompanionController companion, InteractableBase target)
    {
        return PuzzleManager.Instance.IsPuzzleSolved(puzzleKey);
    }
}
