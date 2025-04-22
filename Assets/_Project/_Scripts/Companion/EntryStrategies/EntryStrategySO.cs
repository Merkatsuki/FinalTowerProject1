using UnityEngine;

public abstract class EntryStrategySO : ScriptableObject
{
    public abstract bool CanEnter(IPuzzleInteractor actor, IWorldInteractable target);
}
