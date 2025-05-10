// Refactored SwitchFeature.cs to inherit from PuzzleFeatureBase
using UnityEngine;
using System.Collections.Generic;

public class SwitchFeature : PuzzleFeatureBase
{
    [Header("Switch Settings")]
    [SerializeField] private bool toggleable = true;
    [SerializeField] private bool startOn = false;
    [SerializeField] private bool oneTimeUse = false;
    [SerializeField] private Animator switchAnimator;
    [SerializeField] private string toggleParameter = "On";

    private bool isActivated;
    private bool permanentlyUsed;

    private void Awake()
    {
        isActivated = startOn;
        UpdateVisual();
    }

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (permanentlyUsed) return;
        if (!toggleable && isActivated) return;

        isActivated = toggleable ? !isActivated : true;

        RunFeatureEffects(actor);

        if (oneTimeUse) permanentlyUsed = true;

        UpdateVisual();
        isSolved = true;
        NotifyPuzzleInteractionSuccess();
    }

    private void UpdateVisual()
    {
        if (switchAnimator != null && !string.IsNullOrEmpty(toggleParameter))
        {
            switchAnimator.SetBool(toggleParameter, isActivated);
        }
    }

    public override void ResetPuzzleComponent()
    {
        base.ResetPuzzleComponent();
        isActivated = startOn;
        permanentlyUsed = false;
        UpdateVisual();
    }
}
