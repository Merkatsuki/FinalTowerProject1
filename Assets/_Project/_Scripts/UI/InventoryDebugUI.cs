using UnityEngine;
using TMPro;
using System.Text;

public class InventoryDebugUI : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private TMP_Text inventoryText;

    private void Update()
    {
        if (inventoryText == null || InventoryManager.Instance == null)
            return;

        var items = InventoryManager.Instance.GetAllItems();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<b>Inventory:</b>");
        foreach (var item in items)
        {
            if (item != null)
            {
                sb.AppendLine($"- {item.ItemName}");
            }
        }

        inventoryText.text = sb.ToString();
    }
}
