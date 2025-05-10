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
    [SerializeField] private Button loreViewerOpenButton;

    private void OnEnable()
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

            // Hook story book to lore viewer open button if applicable
            if (item == storyBookItem && loreViewerOpenButton != null)
            {
                loreViewerOpenButton.gameObject.SetActive(true);
                loreViewerOpenButton.onClick.RemoveAllListeners();
                loreViewerOpenButton.onClick.AddListener(OpenLoreViewer);
            }
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
