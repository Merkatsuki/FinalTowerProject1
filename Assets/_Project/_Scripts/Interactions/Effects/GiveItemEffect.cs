using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Give Item Effect")]
public class GiveItemEffect : EffectStrategySO
{
    [SerializeField] private ItemSO itemToGive;

    [SerializeField] private bool onlyOnSuccess = true;

    public void SetItem(ItemSO item) => itemToGive = item;
    public void SetOnlyOnSuccess(bool value) => onlyOnSuccess = value;


    protected override void ApplyEffectInternal(IPuzzleInteractor actor, IWorldInteractable interactable, InteractionResult result)
    {
        if (itemToGive != null)
        {
            InventoryManager.Instance.AddItem(itemToGive);
            Debug.Log($"[Effect] Item '{itemToGive.ItemName}' added to inventory.");
        }
        else
        {
            Debug.LogWarning("[Effect] No item assigned in GiveItemEffect!");
        }
    }
}
