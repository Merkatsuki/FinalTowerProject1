// Refactored PuzzleUnlockFeature.cs to inherit from FeatureBase
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class PuzzleUnlockFeature : FeatureBase
{
    [Header("Unlock Targets")]
    [SerializeField] private List<GameObject> unlockTargets = new();

    [Header("Unlock Flags & Events")]
    [SerializeField] private FlagSO unlockFlag;
    [SerializeField] private UnityEvent onUnlocked;

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (isSolved) return;

        foreach (var target in unlockTargets)
        {
            if (target != null)
                target.SetActive(true);
        }

        if (unlockFlag != null)
            FlagManager.Instance.SetBool(unlockFlag, true);

        onUnlocked?.Invoke();
        RunFeatureEffects(actor);

        isSolved = true;
        NotifyPuzzleInteractionSuccess();
    }

    public override void ResetPuzzleComponent()
    {
        base.ResetPuzzleComponent();

        foreach (var target in unlockTargets)
        {
            if (target != null)
                target.SetActive(false);
        }
    }
}
