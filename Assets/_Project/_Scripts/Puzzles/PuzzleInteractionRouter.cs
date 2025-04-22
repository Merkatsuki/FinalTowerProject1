using UnityEngine;

public static class PuzzleInteractionRouter
{
    public static void HandleInteraction(PuzzleObject target, IPuzzleInteractor actor)
    {
        if (target == null || actor == null) return;

        foreach (var runtime in PuzzleManager.Instance.GetAllActivePuzzleRuntimes())
        {
            runtime.TryProcessInteraction(target, actor);
        }
    }
}