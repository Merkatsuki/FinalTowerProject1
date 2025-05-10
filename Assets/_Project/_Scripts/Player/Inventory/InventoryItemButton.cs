// InventoryItemButton.cs - updated for callback-based interaction
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemButton : MonoBehaviour
{
    [SerializeField] private Text labelText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    private ItemSO assignedItem;
    private System.Action onClickCallback;

    public void Setup(ItemSO item, System.Action callback)
    {
        assignedItem = item;
        onClickCallback = callback;

        if (labelText != null)
            labelText.text = item.ItemName;

        if (iconImage != null && item.Icon != null)
        {
            iconImage.sprite = item.Icon;
            iconImage.enabled = true;
        }
        else if (iconImage != null)
        {
            iconImage.enabled = false;
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClicked);
        }
    }

    private void OnClicked()
    {
        onClickCallback?.Invoke();
    }
}
