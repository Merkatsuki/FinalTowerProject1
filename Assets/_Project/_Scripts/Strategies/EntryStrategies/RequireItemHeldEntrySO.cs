using UnityEngine;

[CreateAssetMenu(menuName = "Interactions/EntryStrategies/RequireItemHeldEntry")]
public class RequireItemHeldEntrySO : EntryStrategySO
{
    [SerializeField] private string requiredItemId;

    public override bool CanEnter(IPuzzleInteractor actor, IWorldInteractable interactable)
    {
        if (string.IsNullOrEmpty(requiredItemId))
            return true;

        return InventoryManager.Instance != null && InventoryManager.Instance.HasItem(requiredItemId);
    }
}

