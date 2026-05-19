using System;
using UnityEngine;

public class resourcesManager : MonoBehaviour
{
    public enum ResourceType { Metal, Combustible, InsumosElectronicos }
    public ResourceType type;
    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
        Debug.Log("Que la parta y la choque");

        if (playerInventory != null )
        {
            playerInventory.CollectResource(type);
            gameObject.SetActive(false);
        }
    }

}
