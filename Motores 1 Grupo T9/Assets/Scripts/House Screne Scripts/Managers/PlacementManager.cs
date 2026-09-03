using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance { get; private set; }

    private string heldItemId;

    public bool HasHeldItem => !string.IsNullOrEmpty(heldItemId);
    public string HeldItemId => heldItemId;

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
        heldItemId = itemId;
    }

    public bool TryConsume(string requiredItemId)
    {
        if (string.IsNullOrEmpty(heldItemId)) return false;
        if (!string.IsNullOrEmpty(requiredItemId) && heldItemId != requiredItemId) return false;

        heldItemId = null;
        return true;
    }
}