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
    [SerializeField] private Image backgroundPanel;
    [SerializeField] private AudioSource pageAudioSource;

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

        if (backgroundPanel != null)
            backgroundPanel.color = item.backgroundTint;

        if (item.pageAudio != null && pageAudioSource != null)
        {
            pageAudioSource.clip = item.pageAudio;
            pageAudioSource.Play();
        }

        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);

        if (pageAudioSource != null)
            pageAudioSource.Stop();
    }
}