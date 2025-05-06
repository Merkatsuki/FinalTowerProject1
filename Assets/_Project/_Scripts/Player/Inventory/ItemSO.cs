using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public enum ItemCategory { StoryPage, Tool, MemoryFragment, Misc }
    public ItemCategory category;

    [Header("Story Page Content")]
    public bool isStoryPage;
    [TextArea(3, 6)]
    public string storyText;
    public Sprite storyImage;
    public AudioClip pageAudio;
    public Color backgroundTint; // Optional theming

    public string ItemID;
    public string ItemName;
    public Sprite Icon;
    [TextArea]
    public string Description;


    
}
