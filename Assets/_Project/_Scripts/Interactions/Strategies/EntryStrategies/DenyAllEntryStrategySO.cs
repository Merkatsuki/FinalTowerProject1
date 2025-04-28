using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Entry/Deny All")]
public class DenyAllEntryStrategySO : EntryStrategySO
{
    public override bool CanEnter(IPuzzleInteractor actor, IWorldInteractable target) => false;
}