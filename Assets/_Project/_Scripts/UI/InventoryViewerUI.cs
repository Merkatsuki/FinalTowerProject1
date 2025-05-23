using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryViewerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject itemButtonPrefab;

    [Header("Special Item Hooks")]
    [SerializeField] private ItemSO storyBookItem;

    private void Start()
    {
        InventoryManager.OnInventoryUpdated += RefreshUI;
        RefreshUI();
    }

    private void OnDisable()
    {
        InventoryManager.OnInventoryUpdated -= RefreshUI;
    }

    private void RefreshUI()
    {
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        List<ItemSO> items = InventoryManager.Instance.GetQuickItems();

        foreach (ItemSO item in items)
        {
            GameObject btnObj = Instantiate(itemButtonPrefab, itemContainer);
            InventoryItemButton button = btnObj.GetComponent<InventoryItemButton>();
            if (button != null)
                button.Setup(item, () => OnItemClicked(item));
        }
    }

    private void OnItemClicked(ItemSO item)
    {
        Debug.Log($"[InventoryViewerUI] Item clicked: {item.ItemName}");

        if (item == storyBookItem)
        {
            OpenLoreViewer();
        }

        // Optional: Add more logic for other item types
    }

    private void OpenLoreViewer()
    {
        LoreViewerUI.Instance?.Open();
    }
}
