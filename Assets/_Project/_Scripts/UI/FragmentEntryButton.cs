using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FragmentEntryButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private Button button;

    private ItemSO assignedItem;
    private System.Action<ItemSO> onClickCallback;

    public void Setup(ItemSO item, System.Action<ItemSO> callback)
    {
        assignedItem = item;
        onClickCallback = callback;

        if (labelText != null)
            labelText.text = item.ItemName;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClickCallback?.Invoke(assignedItem));
        }
    }
}

