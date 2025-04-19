using UnityEngine;

[CreateAssetMenu(menuName = "Companion/EntryStrategies/Require Not Recently Seen")]
public class RequireNotRecentlySeenSO : EntryStrategySO
{
    [SerializeField] private float cooldownTime = 5f;

    public override bool CanEnter(CompanionController companion, CompanionClueInteractable target)
    {
        float lastSeen = companion.GetLastSeenTime(target);
        return Time.time - lastSeen > cooldownTime;
    }
}