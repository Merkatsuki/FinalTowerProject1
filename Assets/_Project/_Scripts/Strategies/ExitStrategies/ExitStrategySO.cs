using UnityEngine;

public abstract class ExitStrategySO : ScriptableObject
{
    public abstract bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target);
    public virtual void OnEnter(IPuzzleInteractor companion, IWorldInteractable target) { }
    public virtual void OnExit(IPuzzleInteractor companion, IWorldInteractable target) { }
}
