using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public string ItemID;
    public string ItemName;
    public Sprite Icon;
    [TextArea]
    public string Description;
    public bool isStoryPage;
    public Sprite storyImage;     // Optional: large image for full-page popup
    [TextArea(5, 20)]
    public string storyText;      // Full body text of the story page
}
