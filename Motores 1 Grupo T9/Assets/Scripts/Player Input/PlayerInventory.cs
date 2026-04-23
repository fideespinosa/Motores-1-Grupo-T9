using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInventory : MonoBehaviour
{
    public int resourcesCollected { get; private set; }

    [SerializeField] private int resourcesNeeded;

    [SerializeField] private TextMeshProUGUI textResources;
    public void CollectResource()
    {
        resourcesCollected++;
        textResources.text = resourcesCollected.ToString();

        if (resourcesCollected.ToString() == resourcesNeeded.ToString())
        {
            SceneManager.LoadScene("Victory");
        }
    }
}
