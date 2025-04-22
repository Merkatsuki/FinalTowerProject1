using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Entry/Require Uncharged State")]
public class RequireUnchargedStateSO : EntryStrategySO
{
    public override bool CanEnter(IPuzzleInteractor actor, IWorldInteractable target)
    {
        return actor.GetEnergyType() == EnergyType.None;
    }
}