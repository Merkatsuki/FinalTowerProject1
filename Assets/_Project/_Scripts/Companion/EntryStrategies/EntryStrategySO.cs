using UnityEngine;

public abstract class EntryStrategySO : ScriptableObject
{
    public abstract bool CanEnter(CompanionController companion, CompanionClueInteractable target);
}
