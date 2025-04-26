using UnityEngine;
using System.Collections.Generic;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private HashSet<string> collectedItems = new HashSet<string>();

    public event Action<string> OnItemAdded;
    public event Action<string> OnItemRemoved;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void AddItem(string itemID)
    {
        if (collectedItems.Add(itemID))
        {
            Debug.Log($"Collected item: {itemID}");
            OnItemAdded?.Invoke(itemID);
        }
    }

    public void RemoveItem(string itemID)
    {
        if (collectedItems.Remove(itemID))
        {
            Debug.Log($"Removed item: {itemID}");
            OnItemRemoved?.Invoke(itemID);
        }
    }

    public bool HasItem(string itemID) => collectedItems.Contains(itemID);

    public List<string> GetAllItems()
    {
        return new List<string>(collectedItems);
    }

    public void ClearInventory()
    {
        collectedItems.Clear();
        Debug.Log("Inventory cleared.");
    }
}
