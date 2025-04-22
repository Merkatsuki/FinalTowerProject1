using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IWorldInteractable
{
    [Header("Strategy Settings")]
    [SerializeField] protected List<EntryStrategySO> entryStrategies;
    [SerializeField] protected List<ExitStrategySO> exitStrategies;

    public virtual string GetDisplayName() => gameObject.name;
    public virtual Transform GetTransform() => transform;

    public virtual bool CanBeInteractedWith(IPuzzleInteractor actor)
    {
        foreach (var strategy in entryStrategies)
        {
            if (strategy != null && !strategy.CanEnter(actor, this))
                return false;
        }
        return true;
    }

    public virtual List<ExitStrategySO> GetExitStrategies() => exitStrategies;

    public abstract void OnInteract(IPuzzleInteractor actor);

    public bool ShouldExit(IPuzzleInteractor actor)
    {
        foreach (var strategy in exitStrategies)
        {
            if (strategy != null && strategy.ShouldExit(actor, this))
                return true;
        }

        return false;
    }
}