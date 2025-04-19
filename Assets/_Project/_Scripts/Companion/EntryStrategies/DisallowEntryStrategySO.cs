using UnityEngine;

[CreateAssetMenu(menuName = "EntryStrategy/Disallow All")]
public class DisallowEntryStrategySO : EntryStrategySO
{
    public override bool CanEnter(CompanionController companion, CompanionClueInteractable target)
    {
        return false;
    }
}
