using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

    [Header("Spawns por recolección")]
    [SerializeField] private List<SpawnTrigger> spawnTriggers = new List<SpawnTrigger>();

    [Header("Anomalía - se activa al completar todos los recursos")]
    [SerializeField] private GameObject anomalyObject;
    [SerializeField] private bool anomalyTriggered = false;

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

        CheckSpawnTriggers(type);
    }

    private int GetCollectedAmount(resourcesManager.ResourceType type)
    {
        switch (type)
        {
            case resourcesManager.ResourceType.Metal: return metalCollected;
            case resourcesManager.ResourceType.Combustible: return combustibleCollected;
            case resourcesManager.ResourceType.InsumosElectronicos: return insumosCollected;
            default: return 0;
        }
    }

    private void CheckSpawnTriggers(resourcesManager.ResourceType type)
    {
        foreach (var trigger in spawnTriggers)
        {
            if (trigger.alreadyTriggered) continue;
            if (trigger.resourceType != type) continue;

            if (GetCollectedAmount(type) >= trigger.amountRequired)
            {
                SpawnEnemies(trigger);
                trigger.alreadyTriggered = true;
            }
        }
    }

    private void SpawnEnemies(SpawnTrigger trigger)
    {
        if (trigger.navMeshEntryPoint == null)
        {
            Debug.LogWarning("SpawnTrigger sin navMeshEntryPoint asignado.");
            return;
        }

        foreach (var enemy in trigger.enemiesToActivate)
        {
            if (enemy == null) continue;

            enemy.SetActive(true);

            var entry = enemy.AddComponent<EnemyWallEntry>();
            entry.Init(trigger.navMeshEntryPoint.position);
        }
    }

    public void CheckVictory()
    {
        Debug.Log("Chequea");

        if (metalCollected >= metalNeeded &&
            combustibleCollected >= combustibleNeeded &&
            insumosCollected >= insumosNeeded)
        {
            TriggerAnomaly();
            return;
        }

        Debug.Log("Faltan materiales");
    }

    private void TriggerAnomaly()
    {
        if (anomalyTriggered) return;

        if (anomalyObject == null)
        {
            Debug.LogWarning("anomalyObject no asignado.");
            return;
        }

        Debug.Log("Anomalía activada.");
        anomalyObject.SetActive(true);
        anomalyTriggered = true;
    }

    public void LevelUp()
    {
    }
}
