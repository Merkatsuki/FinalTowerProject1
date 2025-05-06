using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemButtonPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private StoryPageViewerUI storyViewer;

    private void OnEnable()
    {
        RefreshInventoryUI();
    }

    public void RefreshInventoryUI()
    {
        // Clear existing buttons
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        List<ItemSO> items = InventoryManager.Instance.GetAllItems();
        foreach (ItemSO item in items)
        {
            GameObject newButton = Instantiate(itemButtonPrefab, contentParent);
            TMP_Text label = newButton.GetComponentInChildren<TMP_Text>();
            Image icon = newButton.GetComponentInChildren<Image>();

            if (label != null) label.text = item.ItemName;
            if (icon != null && item.Icon != null) icon.sprite = item.Icon;

            Button btn = newButton.GetComponent<Button>();
            if (btn != null && item.isStoryPage && storyViewer != null)
            {
                btn.onClick.AddListener(() => storyViewer.ShowPage(item));
            }
        }
    }
}