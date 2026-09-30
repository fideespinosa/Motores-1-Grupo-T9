using System;
using UnityEngine;

public class resourcesManager : MonoBehaviour
{
    public enum ResourceType { Metal, Combustible, InsumosElectronicos }
    public ResourceType type;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip stingerClip;
    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
        Debug.Log("colision");

        if (playerInventory != null )
        {
            playerInventory.CollectResource(type);
            audioSource.PlayOneShot(stingerClip);
            gameObject.SetActive(false);
        }
    }

}
