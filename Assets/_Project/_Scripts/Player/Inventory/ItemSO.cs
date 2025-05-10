using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public enum ItemCategory
    {
        StoryPage,
        Tool,
        MemoryFragment,
        DebugToken,
        EmotionEcho
    }

    [Header("Identification")]
    public string ItemID;
    public string ItemName;
    [TextArea] public string Description;

    [Header("Category & Inventory Settings")]
    public ItemCategory category;
    public bool isQuickInventoryItem = true;
    public Sprite Icon;

    [Header("Story Page Content")]
    public bool isStoryPage;
    [TextArea(3, 6)] public string storyText;
    public Sprite storyImage;
    public AudioClip pageAudio;
    public Color backgroundTint;

    [Header("Optional Lore Presentation")]
    public AudioClip audioClip;
    public DialogueSequenceSO dialogueOnInspect;
}

