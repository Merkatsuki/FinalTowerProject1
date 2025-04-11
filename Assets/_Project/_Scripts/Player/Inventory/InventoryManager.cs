using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private HashSet<string> collectedItems = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void AddItem(string itemID)
    {
        collectedItems.Add(itemID);
        Debug.Log($"Collected item: {itemID}");
    }

    public bool HasItem(string itemID) => collectedItems.Contains(itemID);
}
