using UnityEngine;

[CreateAssetMenu(menuName = "Companion/EntryStrategies/Require Player Command")]
public class RequirePlayerCommandSO : EntryStrategySO
{
    public override bool CanEnter(CompanionController companion, CompanionClueInteractable target)
    {
        return companion.WasCommanded(target);
    }
}