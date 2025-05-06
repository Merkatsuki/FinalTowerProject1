using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryPageViewerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Image storyImage;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }
    }

    public void ShowPage(ItemSO item)
    {
        if (item == null || !item.isStoryPage)
        {
            Debug.LogWarning("Tried to show a non-story page item.");
            return;
        }

        titleText.text = item.ItemName;
        bodyText.text = item.storyText;
        storyImage.sprite = item.storyImage;
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}