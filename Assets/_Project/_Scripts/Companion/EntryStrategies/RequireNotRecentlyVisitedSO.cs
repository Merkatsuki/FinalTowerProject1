using UnityEngine;

[CreateAssetMenu(menuName = "EntryStrategy/Require Not Recently Visited")]
public class RequireNotRecentlyVisitedSO : EntryStrategySO
{
    [SerializeField] private string clueId;
    [SerializeField] private float cooldownTime = 10f;

    public override bool CanEnter(CompanionController companion, CompanionClueInteractable target)
    {
        return PuzzleMemoryLog.Instance != null && !PuzzleMemoryLog.Instance.WasClueRecentlyVisited(clueId, cooldownTime);
    }
}



