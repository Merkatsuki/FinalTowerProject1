using System;

public interface IPuzzleComponent
{
    void RegisterToPuzzle(PuzzleController controller);
    void NotifyPuzzleInteractionSuccess();
    void NotifyPuzzleInteractionFailure();
    void ResetPuzzleComponent();
}