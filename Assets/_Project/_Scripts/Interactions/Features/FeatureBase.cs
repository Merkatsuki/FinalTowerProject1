using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unified base for all interaction features, with puzzle support, exit tracking, and sequencing logic.
/// </summary>
public abstract class FeatureBase : MonoBehaviour, IInteractableFeature
{
    protected IWorldInteractable interactable;

    [Header("Puzzle Integration")]
    [SerializeField] protected bool isSolved = false;
    [SerializeField] protected PuzzleController puzzleController;
    [SerializeField] protected List<EffectStrategySO> featureEffects = new();

    [Header("Exit Logic")]
    [SerializeField] protected List<ExitStrategySO> exitStrategies = new();
    protected bool isComplete = false;
    public virtual bool IsComplete => isComplete;

    protected virtual void Awake()
    {
        interactable = GetComponent<IWorldInteractable>();
    }

    public virtual bool CanBeInteractedWith(IPuzzleInteractor interactor) => true;

    public abstract void OnInteract(IPuzzleInteractor interactor);

    public virtual void Tick(IPuzzleInteractor interactor)
    {
        if (isComplete || exitStrategies.Count == 0) return;

        foreach (var exit in exitStrategies)
        {
            if (!exit.ShouldExit(interactor, interactable))
                return;
        }

        isComplete = true;
    }

    public virtual string GetDescription() => name + " (Feature)";

    // --- Puzzle support methods ---

    public virtual void RegisterToPuzzle(PuzzleController controller)
    {
        puzzleController = controller;
    }

    public virtual void NotifyPuzzleInteractionSuccess()
    {
        isSolved = true;
        puzzleController?.ReportComponentSuccess(this);
    }

    public virtual void NotifyPuzzleInteractionFailure()
    {
        puzzleController?.FailPuzzle();
    }

    public virtual void ResetPuzzleComponent()
    {
        isSolved = false;
    }

    protected virtual void RunFeatureEffects(IPuzzleInteractor actor = null)
    {
        foreach (var effect in featureEffects)
        {
            if (effect != null && interactable != null)
            {
                effect.ApplyEffect(actor, interactable, InteractionResult.Success);
            }
        }
    }

    public void SetFeatureEffects(List<EffectStrategySO> effects)
    {
        featureEffects = effects ?? new();
    }

    public bool IsSolved() => isSolved;
}
