using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private readonly HashSet<string> collectedItemIds = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return;
        collectedItemIds.Add(itemId);
    }

    public void RemoveItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return;
        collectedItemIds.Remove(itemId);
    }

    public bool HasItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return false;
        return collectedItemIds.Contains(itemId);
    }
}
