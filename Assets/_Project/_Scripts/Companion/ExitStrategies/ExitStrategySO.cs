using UnityEngine;

public abstract class ExitStrategySO : ScriptableObject
{
    public abstract bool ShouldExit(CompanionController companion, CompanionClueInteractable target);
    public virtual void OnEnter(CompanionController companion, CompanionClueInteractable target) { }
}
