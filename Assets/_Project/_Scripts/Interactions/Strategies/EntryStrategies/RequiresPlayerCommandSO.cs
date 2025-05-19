using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Entry/Requires Player Command")]
public class RequiresPlayerCommandSO : EntryStrategySO
{
    public override bool CanEnter(IPuzzleInteractor actor, IWorldInteractable target)
    {
        return actor is CompanionController companion;
    }
}


