using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Entry/RequireItemHeldEntry")]
public class RequireItemHeldEntrySO : EntryStrategySO
{
    [SerializeField] private string requiredItemId;

    public override bool CanEnter(IPuzzleInteractor actor, IWorldInteractable interactable)
    {
        if (string.IsNullOrEmpty(requiredItemId))
            return true;

        return InventoryManager.Instance != null && InventoryManager.Instance.HasItem(requiredItemId);
    }
    public void SetRequiredItemId(string itemId)
    {
        requiredItemId = itemId;
    }

}

