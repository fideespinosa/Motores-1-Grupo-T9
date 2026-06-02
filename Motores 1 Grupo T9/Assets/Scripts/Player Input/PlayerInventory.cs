using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerInventory : MonoBehaviour
{
    public int resourcesCollected { get; private set; }

    [Header("Recursos recolectados")]
    public int metalCollected;
    public int combustibleCollected;
    public int insumosCollected;

    [Header("Recursos necesarios para ganar")]
    [SerializeField] public int metalNeeded = 4;
    [SerializeField] public int combustibleNeeded = 3;
    [SerializeField] public int insumosNeeded = 3;

    [Header("Siguiente Nivel")]
    [SerializeField] public string nextLevel;

    [Header("GUI - Dron")]
    [SerializeField] private TextMeshProUGUI textMetal;
    [SerializeField] private TextMeshProUGUI textCombustible;
    [SerializeField] private TextMeshProUGUI textInsumos;

    [Header("GUI - Player")]
    [SerializeField] private TextMeshProUGUI textPMetal;
    [SerializeField] private TextMeshProUGUI textPCombustible;
    [SerializeField] private TextMeshProUGUI textPInsumos;

    public void CollectResource(resourcesManager.ResourceType type)
    {

        switch (type)
        {
            case resourcesManager.ResourceType.Metal:
                metalCollected++;
                textMetal.text = metalCollected.ToString();
                textPMetal.text = metalCollected.ToString();
                break;

            case resourcesManager.ResourceType.Combustible:
                combustibleCollected++;
                textCombustible.text = combustibleCollected.ToString();
                textPCombustible.text = combustibleCollected.ToString();
                break;

            case resourcesManager.ResourceType.InsumosElectronicos:
                insumosCollected++;
                textInsumos.text = insumosCollected.ToString();
                textPInsumos.text = insumosCollected.ToString();
                break;
        }
    }

    public void CheckVictory()
    {
        Debug.Log("Chequea");
        
        if (metalCollected >= metalNeeded &&
            combustibleCollected >= combustibleNeeded &&
            insumosCollected >= insumosNeeded)
        {
            SceneManager.LoadScene(nextLevel);
            return;
        }

        Debug.Log("Faltan materiales");

    }

    public void LevelUp()
    {
    }
}
