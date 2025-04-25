using UnityEngine;

public class CollectibleInteractable : InteractableBase
{
    [SerializeField] private string itemId;

    public override void OnInteract(IPuzzleInteractor actor)
    {
        InventoryManager.Instance.AddItem(itemId);
        Debug.Log($"{actor.GetDisplayName()} collected {itemId}");
        Destroy(gameObject);
    }
}