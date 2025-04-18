using UnityEngine;

public abstract class ExitStrategySO : ScriptableObject
{
    public abstract bool ShouldExit(CompanionController companion, InteractableBase target);
    public virtual void OnEnter(CompanionController companion, InteractableBase target) { }
}
