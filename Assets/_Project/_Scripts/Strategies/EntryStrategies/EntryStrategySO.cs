using UnityEngine;

public abstract class EntryStrategySO : ScriptableObject
{
    public abstract bool CanEnter(IPuzzleInteractor actor, IWorldInteractable target);
    public virtual void OnEnter(IPuzzleInteractor companion, IWorldInteractable target) { }
    public virtual void OnExit(IPuzzleInteractor companion, IWorldInteractable target) { }
}
