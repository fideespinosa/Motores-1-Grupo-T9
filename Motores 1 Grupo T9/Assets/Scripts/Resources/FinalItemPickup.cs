using UnityEngine;

public class FinalItemPickup : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip stingerClip;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        inventory.CollectFinalItem();
        audioSource.clip = stingerClip;
        audioSource.Play();
    }
}