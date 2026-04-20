using System;
using UnityEngine;

public class resourcesManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
        Debug.Log("Que la parta y la choque");

        if (playerInventory != null )
        {
            playerInventory.CollectResource();
            gameObject.SetActive(false);
        }
    }

}
