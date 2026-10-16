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
    public int anomaliaCollected;

    [Header("Recursos necesarios para ganar")]
    [SerializeField] public int metalNeeded = 4;
    [SerializeField] public int combustibleNeeded = 3;
    [SerializeField] public int insumosNeeded = 3;

    [Header("Objeto final")]
    [SerializeField] private GameObject finalItem; // objeto en escena, desactivado por defecto
    private bool finalItemActivated;

    [Header("Spawns por recolección")]
    [SerializeField] private List<SpawnTrigger> spawnTriggers = new List<SpawnTrigger>();

    [Header("Anomalía - se activa al completar todos los recursos")]
    [SerializeField] private GameObject anomalyObject;
    [SerializeField] private GameObject anomalyTrigger;
    [SerializeField] private GameObject enemies;
    [SerializeField] private GameObject wallCaveBlock;
    [SerializeField] private bool anomalyTriggered = false;
    [SerializeField] private float anomalyDelay = 3f;
    [SerializeField] private CanvasGroup whitePanel;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("SFX Ref")]
    [SerializeField] private ShipVoiceManager shipVoiceManager;


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
    private bool finalItemCollected;

    private bool AllResourcesDone =>
        metalCollected >= metalNeeded &&
        combustibleCollected >= combustibleNeeded &&
        insumosCollected >= insumosNeeded;

    // Llamala también al volver a la nave/dron (ver paso 4)
    public void RefreshUI()
    { 
        // Contadores simples
        if (textMetal) textMetal.text = metalCollected.ToString();
        if (textCombustible) textCombustible.text = combustibleCollected.ToString();
        if (textInsumos) textInsumos.text = insumosCollected.ToString();

        // Panel de la nave
        if (textPMetal) textPMetal.text = metalCollected.ToString();
        if (textPCombustible) textPCombustible.text = combustibleCollected.ToString();
        if (textPInsumos) textPInsumos.text = insumosCollected.ToString();

        if (camDrone == null || camDrone.qResources1Lbl == null) return;

        if (!AllResourcesDone)
        {
            camDrone.qResources1Lbl.text = metalCollected.ToString();
            camDrone.qResources2Lbl.text = combustibleCollected.ToString();
            camDrone.qResources3Lbl.text = insumosCollected.ToString();
            return;
        }

        // Todo recolectado: limpia y muestra el objetivo final
        camDrone.resources1Lbl.text = "";
        camDrone.resources2Lbl.text = "";
        camDrone.resources3Lbl.text = "";
        camDrone.qResources1Lbl.text = "";
        camDrone.qResources2Lbl.text = "";
        camDrone.qResources3Lbl.text = "";

        if (finalItemActivated || finalItemCollected)
        {
            camDrone.resources1Lbl.text = " / 1 Anomalia";
            camDrone.qResources1Lbl.text = finalItemCollected ? "1" : "0";
        }
    }

    public void CollectResource(resourcesManager.ResourceType type)
    {
        switch (type)
        {
            case resourcesManager.ResourceType.Metal:
                metalCollected++;
                break;
            case resourcesManager.ResourceType.Combustible:
                combustibleCollected++;
                break;
            case resourcesManager.ResourceType.InsumosElectronicos:
                insumosCollected++;
                break;
        }
        RefreshUI();
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

        shipVoiceManager.FisrtEnemy();
    }

    public void CheckVictory()
    {
        Debug.Log($"[CheckVictory] metal={metalCollected}/{metalNeeded}, combustible={combustibleCollected}/{combustibleNeeded}, insumos={insumosCollected}/{insumosNeeded}");

        if (!AllResourcesDone) return;

        if (finalItem != null)
        {
            if (!finalItemActivated)
            {
                finalItemActivated = true;
                finalItem.SetActive(true);
                Debug.Log("[CheckVictory] recursos completos, objeto final activado");
            }
            RefreshUI();
            return;
        }

       
        RefreshUI();
        TriggerAnomaly();
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

        yield return StartCoroutine(FadePanel(0f, 1f));

        anomalyTrigger.SetActive(true);
        enemies.SetActive(false);
        anomalyObject.SetActive(true);
        wallCaveBlock.SetActive(false); // se desactiva la pared de piedra que bloqueaba la salida cuando no estaba la puerta de la casa en la cueva

        yield return StartCoroutine(FadePanel(1f, 0f));

        whitePanel.gameObject.SetActive(false);
        whitePanel.alpha = 1f; // lo pongo en 1 porque en otro script se usa este panel pero no tiene la logica de ponerlo en 1 al panel
    }

    private IEnumerator FadePanel(float startAlpha, float targetAlpha)
    {
        whitePanel.gameObject.SetActive(true);
        whitePanel.alpha = startAlpha;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / fadeDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            whitePanel.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        whitePanel.alpha = targetAlpha;
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
    public void CollectFinalItem()
    {
        if (!finalItemActivated || finalItemCollected) return;

        if (finalItem != null) finalItem.SetActive(false);
        finalItemCollected = true;
        RefreshUI();
        TriggerAnomaly();
    }

    public void LevelUp()
    {
    }
}