using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class LoreViewerUI : MonoBehaviour
{
    public static LoreViewerUI Instance { get; private set; }

    [Header("Mode Buttons")]
    [SerializeField] private Button storyPageModeButton;
    [SerializeField] private Button memoryFragmentModeButton;

    [Header("Story Page Controls")]
    [SerializeField] private GameObject partButtonGroup;
    [SerializeField] private List<Button> storyPartButtons;

    [Header("Fragment Viewer")]
    [SerializeField] private GameObject fragmentScrollView;
    [SerializeField] private Transform fragmentContent;
    [SerializeField] private GameObject fragmentEntryPrefab;

    [Header("Book Pages")]
    [SerializeField] private TextMeshProUGUI leftPageText;
    [SerializeField] private TextMeshProUGUI rightPageText;

    private enum ViewMode { Story, Fragment }
    private ViewMode currentMode = ViewMode.Story;

    private List<ItemSO> storyPages = new();
    private List<ItemSO> fragments = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        storyPageModeButton.onClick.AddListener(() => SwitchMode(ViewMode.Story));
        memoryFragmentModeButton.onClick.AddListener(() => SwitchMode(ViewMode.Fragment));

        for (int i = 0; i < storyPartButtons.Count; i++)
        {
            int index = i;
            storyPartButtons[i].onClick.AddListener(() => DisplayStoryPart(index));
        }
    }

    private void Update()
    {
        if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }


    public void Open()
    {
        gameObject.SetActive(true);
        LoadItems();
        SwitchMode(ViewMode.Fragment);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void LoadItems()
    {
        Debug.Log($"[LoreViewer] Found {storyPages.Count} story pages.");
        foreach (var page in storyPages)
            Debug.Log($"Story Page: {page.ItemName}, ID: {page.ItemID}");

        storyPages = InventoryManager.Instance.GetAllItems()
            .Where(i => i.category == ItemSO.ItemCategory.StoryPage)
            .OrderBy(i => i.ItemID)
            .ToList();

        fragments = InventoryManager.Instance.GetAllItems()
            .Where(i => i.category == ItemSO.ItemCategory.MemoryFragment)
            .OrderBy(i => i.ItemID)
            .ToList();
    }

    private void SwitchMode(ViewMode mode)
    {
        LoadItems();

        currentMode = mode;
        partButtonGroup.SetActive(mode == ViewMode.Story);
        fragmentScrollView.SetActive(mode == ViewMode.Fragment);

        ClearPages();

        if (mode == ViewMode.Story)
            DisplayStoryPart(0);
        else
            PopulateFragmentList();
    }

    private void DisplayStoryPart(int index)
    {
        LoadItems();
        if (index < 0 || index >= storyPages.Count)
        {
            leftPageText.text = "";
            rightPageText.text = "";
            return;
        }

        ItemSO page = storyPages[index];
        leftPageText.text = page.storyLeftPageText;
        rightPageText.text = page.storyRightPageText;
    }

    private void PopulateFragmentList()
    {
        foreach (Transform child in fragmentContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var fragment in fragments)
        {
            GameObject entryObj = Instantiate(fragmentEntryPrefab, fragmentContent);
            FragmentEntryButton button = entryObj.GetComponent<FragmentEntryButton>();
            if (button != null)
                button.Setup(fragment, ShowFragmentDetail);
        }

        leftPageText.text = "";
        rightPageText.text = "";
    }

    private void ShowFragmentDetail(ItemSO fragment)
    {
        leftPageText.text = "";
        rightPageText.text = fragment.Description;
    }

    private void ClearPages()
    {
        leftPageText.text = "";
        rightPageText.text = "";
    }
}
