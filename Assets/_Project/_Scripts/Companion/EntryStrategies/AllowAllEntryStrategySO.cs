using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Entry/Allow All")]
public class AllowAllEntryStrategySO : EntryStrategySO
{
    public override bool CanEnter(IPuzzleInteractor actor, IWorldInteractable target) => true;
}