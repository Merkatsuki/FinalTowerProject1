using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemButtonPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private StoryPageViewerUI storyViewer;
    [SerializeField] private TMP_Text collectionProgressText;
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMP_Text tooltipText;
    [SerializeField] private Toggle storyPageFilterToggle;

    private void OnEnable()
    {
        RefreshInventoryUI();
    }

    public void RefreshInventoryUI()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        List<ItemSO> allItems = InventoryManager.Instance.GetAllItems();
        int storyPagesCollected = 0;

        foreach (ItemSO item in allItems)
        {
            if (item.isStoryPage) storyPagesCollected++;
        }

        foreach (ItemSO item in allItems)
        {
            if (storyPageFilterToggle != null && storyPageFilterToggle.isOn && !item.isStoryPage)
                continue;

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

            EventTrigger trigger = newButton.AddComponent<EventTrigger>();
            var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };

            entryEnter.callback.AddListener((e) => ShowTooltip(item.Description));
            entryExit.callback.AddListener((e) => HideTooltip());

            trigger.triggers.Add(entryEnter);
            trigger.triggers.Add(entryExit);
        }

        if (collectionProgressText != null)
            collectionProgressText.text = $"Story Pages: {storyPagesCollected}/4";
    }

    private void ShowTooltip(string description)
    {
        if (tooltipPanel != null && tooltipText != null)
        {
            tooltipText.text = description;
            tooltipPanel.SetActive(true);
        }
    }

    private void HideTooltip()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }
}