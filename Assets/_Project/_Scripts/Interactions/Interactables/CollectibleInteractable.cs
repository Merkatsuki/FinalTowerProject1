using UnityEngine;

public class CollectibleInteractable : InteractableBase
{
    [SerializeField] private GameObject highlightVisual;
    [SerializeField] private GameObject pickupEffect;
    [SerializeField] private string itemID = "Key01";

    public override void OnFocusEnter() => SetHighlighted(true);
    public override void OnFocusExit() => SetHighlighted(false);

    public override void SetHighlighted(bool isHighlighted)
    {
        if (highlightVisual != null)
            highlightVisual.SetActive(isHighlighted);
    }

    public override void OnInteract()
    {
        InventoryManager.Instance?.AddItem(itemID);

        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}