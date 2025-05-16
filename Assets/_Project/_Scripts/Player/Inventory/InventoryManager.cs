using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private List<ItemSO> debugAddOnStart;

    private readonly HashSet<string> collectedItems = new();
    private readonly Dictionary<string, ItemSO> itemDatabase = new();

    public static event Action<string> OnItemAdded;
    public static event Action OnInventoryUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        foreach (var item in debugAddOnStart)
        {
            AddItem(item);
        }
    }

    public void RegisterItem(ItemSO item)
    {
        if (item == null || string.IsNullOrEmpty(item.ItemID)) return;

        if (!itemDatabase.ContainsKey(item.ItemID))
            itemDatabase.Add(item.ItemID, item);
    }

    public void AddItem(ItemSO item)
    {
        Debug.Log($"[InventoryManager] Adding item: {item.ItemName} (ID: {item.ItemID})");

        if (item == null || string.IsNullOrEmpty(item.ItemID)) return;

        if (collectedItems.Contains(item.ItemID)) return;

        collectedItems.Add(item.ItemID);
        RegisterItem(item);

        OnItemAdded?.Invoke(item.ItemID);
        OnInventoryUpdated?.Invoke();
    }

    public List<ItemSO> GetAllItems()
    {
        List<ItemSO> result = new();
        foreach (var id in collectedItems)
        {
            if (itemDatabase.TryGetValue(id, out var item))
                result.Add(item);
        }
        return result;
    }

    public List<ItemSO> GetQuickItems()
    {
        List<ItemSO> result = new();
        foreach (var id in collectedItems)
        {
            if (itemDatabase.TryGetValue(id, out var item) && item.isQuickInventoryItem)
                result.Add(item);
        }
        return result;
    }

    public bool HasItem(string itemId)
    {
        return collectedItems.Contains(itemId);
    }

    public bool HasItem(ItemSO item)
    {
        return item != null && collectedItems.Contains(item.ItemID);
    }


    [ContextMenu("Print Collected Inventory Items")]
    public void DebugPrintInventory()
    {
        Debug.Log("[InventoryManager] --- INVENTORY DEBUG START ---");
        foreach (var id in collectedItems)
        {
            if (itemDatabase.TryGetValue(id, out var item))
            {
                Debug.Log($"Collected Item: {item.ItemName} (ID: {item.ItemID}, Category: {item.category})");
            }
            else
            {
                Debug.LogWarning($"Collected item ID '{id}' has no matching ItemSO in database!");
            }
        }
        Debug.Log("[InventoryManager] --- INVENTORY DEBUG END ---");
    }


}
