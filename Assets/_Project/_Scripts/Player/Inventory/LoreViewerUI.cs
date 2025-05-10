// LoreViewerUI.cs - with audio and dialogue playback
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using static ItemSO;
using Unity.VisualScripting;

public class LoreViewerUI : MonoBehaviour
{
    public static LoreViewerUI Instance { get; private set; }

    [Header("Tabs")]
    [SerializeField] private Button storyPageTabButton;
    [SerializeField] private Button memoryFragmentTabButton;
    [SerializeField] private Button echoTabButton;

    [Header("UI Panels")]
    [SerializeField] private GameObject contentContainer;
    [SerializeField] private Transform itemGrid;
    [SerializeField] private GameObject loreItemButtonPrefab;

    [Header("Detail View")]
    [SerializeField] private GameObject detailPanel;
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Image itemImage;
    [SerializeField] private Button playAudioButton;
    [SerializeField] private Button playDialogueButton;
    [SerializeField] private Button closeButton;

    private AudioSource audioSource;
    private ItemSO currentItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gameObject.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        RefreshCategory(currentCategory);
    }

    private void Start()
    {
        storyPageTabButton.onClick.AddListener(() => RefreshCategory(ItemCategory.StoryPage));
        memoryFragmentTabButton.onClick.AddListener(() => RefreshCategory(ItemCategory.MemoryFragment));
        echoTabButton.onClick.AddListener(() => RefreshCategory(ItemCategory.EmotionEcho));
        closeButton.onClick.AddListener(Close);
        playAudioButton.onClick.AddListener(PlayAudio);
        playDialogueButton.onClick.AddListener(PlayDialogue);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        RefreshCategory(currentCategory);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    private ItemCategory currentCategory = ItemCategory.StoryPage;

    private void RefreshCategory(ItemCategory category)
    {
        currentCategory = category;

        foreach (Transform child in itemGrid)
        {
            Destroy(child.gameObject);
        }

        List<ItemSO> items = InventoryManager.Instance.GetAllItems()
            .Where(i => i.category == category).ToList();

        foreach (ItemSO item in items)
        {
            GameObject buttonObj = Instantiate(loreItemButtonPrefab, itemGrid);
            LoreItemButton button = buttonObj.GetComponent<LoreItemButton>();
            if (button != null)
                button.Setup(item, ShowDetail);
        }

        detailPanel.SetActive(false);
    }

    private void ShowDetail(ItemSO item)
    {
        if (item == null) return;

        currentItem = item;
        titleText.text = item.ItemName;
        descriptionText.text = item.Description;
        itemImage.sprite = item.Icon;
        itemImage.gameObject.SetActive(item.Icon != null);

        playAudioButton.gameObject.SetActive(item.audioClip != null);
        playDialogueButton.gameObject.SetActive(item.dialogueOnInspect != null);

        detailPanel.SetActive(true);
    }

    private void PlayAudio()
    {
        if (currentItem?.audioClip != null)
        {
            audioSource.Stop();
            audioSource.clip = currentItem.audioClip;
            audioSource.Play();
        }
    }

    private void PlayDialogue()
    {
        if (currentItem?.dialogueOnInspect != null)
        {
            DialogueManager.Instance?.PlaySequence(currentItem.dialogueOnInspect);
        }
    }
}
