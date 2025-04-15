using UnityEngine;

public abstract class RobotInteractionSO : ScriptableObject
{
    public abstract void Execute(CompanionController companion, InteractableBase target);

    public virtual bool ShouldRemainAvailable() => false;
}

