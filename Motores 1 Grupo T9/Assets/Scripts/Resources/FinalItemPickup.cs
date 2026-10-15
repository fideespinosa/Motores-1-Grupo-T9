using UnityEngine;

public class FinalItemPickup : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        inventory.CollectFinalItem();
    }
}