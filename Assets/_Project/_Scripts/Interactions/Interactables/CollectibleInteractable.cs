using UnityEngine;

public class CollectibleInteractable : InteractableBase
{
    [SerializeField] private GameObject highlightVisual;
    [SerializeField] private GameObject pickupEffect;

    public override void OnFocusEnter() => SetHighlighted(true);
    public override void OnFocusExit() => SetHighlighted(false);

    public override void SetHighlighted(bool isHighlighted)
    {
        if (highlightVisual != null)
            highlightVisual.SetActive(isHighlighted);
    }

    public override void OnInteract()
    {
        Debug.Log("Item Collected: " + gameObject.name);

        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        // You can add inventory logic here later
        Destroy(gameObject);
    }
}