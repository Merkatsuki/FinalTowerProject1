using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public string ItemID;
    public string ItemName;
    public Sprite Icon;
    [TextArea]
    public string Description;
}