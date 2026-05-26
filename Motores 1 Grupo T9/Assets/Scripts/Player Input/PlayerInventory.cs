using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInventory : MonoBehaviour
{
    public int resourcesCollected { get; private set; }

    [Header("Recursos recolectados")]
    public int metalCollected;
    public int combustibleCollected;
    public int insumosCollected;

    [Header("Recursos necesarios para ganar")]
    [SerializeField] private int metalNeeded = 4;
    [SerializeField] private int combustibleNeeded = 3;
    [SerializeField] private int insumosNeeded = 3;

    [Header("GUI")]
    [SerializeField] private TextMeshProUGUI textMetal;
    [SerializeField] private TextMeshProUGUI textCombustible;
    [SerializeField] private TextMeshProUGUI textInsumos;

    public void CollectResource(resourcesManager.ResourceType type)
    {

        switch (type)
        {
            case resourcesManager.ResourceType.Metal:
                metalCollected++;
                textMetal.text = metalCollected.ToString();
                break;

            case resourcesManager.ResourceType.Combustible:
                combustibleCollected++;
                textCombustible.text = combustibleCollected.ToString();
                break;

            case resourcesManager.ResourceType.InsumosElectronicos:
                insumosCollected++;
                textInsumos.text = insumosCollected.ToString();
                break;
        }

        if (metalCollected >= metalNeeded &&
            combustibleCollected >= combustibleNeeded &&
            insumosCollected >= insumosNeeded && SceneManager.GetActiveScene().name == "Level0")
        {
            Debug.Log("Ganó el lvl 0");
            SceneManager.LoadScene("Level1");
            return;
        }

        if (metalCollected >= metalNeeded &&
            combustibleCollected >= combustibleNeeded &&
            insumosCollected >= insumosNeeded)
        {
            SceneManager.LoadScene("Victory");
        }

    }
}
