using UnityEngine;
using System.Collections.Generic;

public abstract class PuzzleFeatureBase : MonoBehaviour, IInteractableFeature, IPuzzleComponent
{
    [Header("Puzzle Integration")]
    [SerializeField] protected bool isSolved = false;
    [SerializeField] protected PuzzleController puzzleController;
    [SerializeField] protected List<EffectStrategySO> featureEffects = new();

    public virtual void RegisterToPuzzle(PuzzleController controller)
    {
        puzzleController = controller;
    }

    public virtual void NotifyPuzzleInteractionSuccess()
    {
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
            if (effect != null && TryGetComponent(out IWorldInteractable interactable))
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

    // Must be implemented by child class
    public abstract void OnInteract(IPuzzleInteractor actor);
}
