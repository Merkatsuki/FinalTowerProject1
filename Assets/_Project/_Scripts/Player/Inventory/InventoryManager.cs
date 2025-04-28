using UnityEngine;
using System.Collections.Generic;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private HashSet<string> collectedItems = new HashSet<string>();
    private Dictionary<string, ItemSO> itemDatabase = new Dictionary<string, ItemSO>();

    public event Action<string> OnItemAdded;
    public event Action<string> OnItemRemoved;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void AddItem(ItemSO item)
    {
        if (item == null) return;

        if (collectedItems.Add(item.ItemID))
        {
            itemDatabase[item.ItemID] = item;
            Debug.Log($"[Inventory] Added item: {item.ItemName}");
            OnItemAdded?.Invoke(item.ItemID);
        }
    }

    public void RemoveItem(ItemSO item)
    {
        if (item == null) return;

        if (collectedItems.Remove(item.ItemID))
        {
            itemDatabase.Remove(item.ItemID);
            Debug.Log($"[Inventory] Removed item: {item.ItemName}");
            OnItemRemoved?.Invoke(item.ItemID);
        }
    }

    public bool HasItem(ItemSO item)
    {
        if (item == null) return false;
        return collectedItems.Contains(item.ItemID);
    }

    public bool HasItem(string itemID)
    {
        return collectedItems.Contains(itemID);
    }

    public void AddItem(string itemID)
    {
        if (collectedItems.Add(itemID))
        {
            Debug.Log($"[Inventory] Added item by ID: {itemID}");
            OnItemAdded?.Invoke(itemID);
        }
    }

    public void RemoveItem(string itemID)
    {
        if (collectedItems.Remove(itemID))
        {
            itemDatabase.Remove(itemID);
            Debug.Log($"[Inventory] Removed item by ID: {itemID}");
            OnItemRemoved?.Invoke(itemID);
        }
    }

    public List<ItemSO> GetAllItems()
    {
        List<ItemSO> items = new List<ItemSO>();
        foreach (var id in collectedItems)
        {
            if (itemDatabase.TryGetValue(id, out var item))
                items.Add(item);
        }
        return items;
    }

    public void ClearInventory()
    {
        collectedItems.Clear();
        itemDatabase.Clear();
        Debug.Log("[Inventory] Inventory cleared.");
    }
}
