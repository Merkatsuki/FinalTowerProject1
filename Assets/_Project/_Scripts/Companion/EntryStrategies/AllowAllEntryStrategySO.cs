using UnityEngine;

[CreateAssetMenu(menuName = "EntryStrategy/Allow All")]
public class AllowAllEntryStrategySO : EntryStrategySO
{
    public override bool CanEnter(CompanionController companion, CompanionClueInteractable target)
    {
        return true;
    }
}
