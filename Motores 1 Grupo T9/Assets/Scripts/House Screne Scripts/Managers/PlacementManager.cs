using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance { get; private set; }

    private readonly HashSet<string> heldItemIds = new HashSet<string>();

    public bool HasHeldItem => heldItemIds.Count > 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PickUp(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return;
        heldItemIds.Add(itemId);
    }

    public bool IsHolding(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return false;
        return heldItemIds.Contains(itemId);
    }

    public string GetHeldItemsDebugString()
    {
        if (heldItemIds.Count == 0) return "(nada)";
        return string.Join(", ", heldItemIds);
    }

    public bool TryConsume(string requiredItemId)
    {
        if (heldItemIds.Count == 0) return false;

        if (!string.IsNullOrEmpty(requiredItemId))
        {
            if (!heldItemIds.Contains(requiredItemId)) return false;

            heldItemIds.Remove(requiredItemId);
            return true;
        }

        string anyItemId = null;
        foreach (string id in heldItemIds)
        {
            anyItemId = id;
            break;
        }

        heldItemIds.Remove(anyItemId);
        return true;
    }
}