using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    public int resourcesCollected { get; private set; }


    public void CollectResource()
    {
        resourcesCollected++;
    }
}
