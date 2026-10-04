using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

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
    [SerializeField] private GameObject anomalyTrigger;
    [SerializeField] private GameObject enemies;
    [SerializeField] private bool anomalyTriggered = false;
    [SerializeField] private float anomalyDelay = 3f;


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
    [SerializeField] private CamDroneUI camDrone;

    [Header("Linterna del dron")]
    [SerializeField] private Light droneFlashlight;
    [SerializeField] private float flickerDuration = 2.5f;
    [SerializeField] private Vector2 flickerInterval = new Vector2(0.03f, 0.15f);
    private Coroutine flickerRoutine;
    private float baseIntensity;

    private void Awake()
    {
        if (droneFlashlight != null)
            baseIntensity = droneFlashlight.intensity;
    }

    public void CollectResource(resourcesManager.ResourceType type)
    {
        switch (type)
        {
            case resourcesManager.ResourceType.Metal:
                metalCollected++;
                //textMetal.text = metalCollected.ToString();
                //camDrone.resources1Lbl.text = $"/ " +  metalCollected.ToString() + "  Piezas de Metal";
                //textPMetal.text = metalCollected.ToString();
                camDrone.qResources1Lbl.text = metalCollected.ToString();
                break;

            case resourcesManager.ResourceType.Combustible:
                combustibleCollected++;
                //textCombustible.text = combustibleCollected.ToString();
                //camDrone.resources2Lbl.text = $"/" + metalCollected.ToString() + "  Posible Combustible";
                //textPCombustible.text = combustibleCollected.ToString();
                camDrone.qResources2Lbl.text = combustibleCollected.ToString();
                break;

            case resourcesManager.ResourceType.InsumosElectronicos:
                insumosCollected++;
                //textInsumos.text = insumosCollected.ToString();
                //camDrone.resources3Lbl.text = $"/" + metalCollected.ToString() + "  Insumos Electrónicos";
                //textPInsumos.text = insumosCollected.ToString();
                camDrone.qResources3Lbl.text = insumosCollected.ToString();
                break;
        }

        CheckSpawnTriggers(type);
        CheckVictory();
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
        }

        FlickerFlashlight();
    }

    public void CheckVictory()
    {
        Debug.Log($"[CheckVictory] metal={metalCollected}/{metalNeeded}, combustible={combustibleCollected}/{combustibleNeeded}, insumos={insumosCollected}/{insumosNeeded}");

        if (metalCollected >= metalNeeded &&
            combustibleCollected >= combustibleNeeded &&
            insumosCollected >= insumosNeeded)
        {
            Debug.Log("[CheckVictory] condición cumplida, llamando TriggerAnomaly");
            TriggerAnomaly();
            return;
        }

        Debug.Log("Faltan materiales");
    }

    private void TriggerAnomaly()
    {
        Debug.Log($"[TriggerAnomaly] anomalyTriggered={anomalyTriggered}, anomalyObject={(anomalyObject != null ? anomalyObject.name : "NULL")}");

        if (anomalyTriggered) return;

        if (anomalyObject == null)
        {
            Debug.LogWarning("[TriggerAnomaly] anomalyObject no asignado.");
            return;
        }

        anomalyTriggered = true;
        StartCoroutine(ActivateAnomalyAfterDelay());
    }

    private IEnumerator ActivateAnomalyAfterDelay()
    {
        yield return new WaitForSeconds(anomalyDelay);
        Debug.Log("[TriggerAnomaly] Anomalía activada, SetActive(true) ejecutado.");
        anomalyObject.SetActive(true);
        anomalyTrigger.SetActive(true);
        enemies.SetActive(false);
    }

    public void FlickerFlashlight()
    {
        if (droneFlashlight == null) return;

        if (flickerRoutine != null)
        {
            StopCoroutine(flickerRoutine);
            droneFlashlight.intensity = baseIntensity;
        }

        flickerRoutine = StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        float timer = 0f;

        while (timer < flickerDuration)
        {
            float wait = Random.Range(flickerInterval.x, flickerInterval.y);
            droneFlashlight.intensity = Random.value > 0.5f ? baseIntensity : baseIntensity * Random.Range(0f, 0.2f);
            yield return new WaitForSeconds(wait);
            timer += wait;
        }

        droneFlashlight.intensity = baseIntensity;
        flickerRoutine = null;
    }

    public void LevelUp()
    {
    }
}