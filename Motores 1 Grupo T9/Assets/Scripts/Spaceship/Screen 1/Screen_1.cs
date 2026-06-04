using UnityEngine;



public class Screen_1 : MonoBehaviour
{
    [SerializeField] private ScreensManagerScript screensManager;

    [Header("UI Despliegue de Dron (Canvas 3D)")]
    [SerializeField] private Canvas deployCanvas;

    [Header("Referencias del Dron en Escena")]
    [SerializeField] private GameObject droneInScene; // ¡Devuelto!
    [SerializeField] private Transform spawnPoint;     // ¡Devuelto!

    [Header("Referencias de Cámara y Player")]
    [SerializeField] private Camera astronautCamera;
    [SerializeField] private GameObject player;

    private PlayerSwitcher switcher;
    private bool isDroneDeployed = false;
    private bool isPlayerLooking = false;

    void Start()
    {
        if (screensManager == null)
        {
            screensManager = Object.FindFirstObjectByType<ScreensManagerScript>();
        }
        else
        {
            screensManager = screensManager.GetComponent<ScreensManagerScript>();
        }

        switcher = Object.FindFirstObjectByType<PlayerSwitcher>();

        if (astronautCamera == null) astronautCamera = Camera.main;
        if (deployCanvas != null) deployCanvas.gameObject.SetActive(false);
    }

    private void OnMouseEnter()
    {
        if (isDroneDeployed) return;
        isPlayerLooking = true;
    }

    private void OnMouseExit()
    {
        isPlayerLooking = false;
    }

    private void Update()
    {
        if (deployCanvas != null && deployCanvas.gameObject.activeSelf && Input.GetMouseButtonDown(0))
        {
            Detect3DUIButtonClick();
        }
    }

    private void OnMouseDown()
    {
        if (isPlayerLooking && !isDroneDeployed && !deployCanvas.gameObject.activeSelf)
        {
            ShowDeploymentOptions();
        }
    }

    public void ShowDeploymentOptions()
    {
        if (deployCanvas != null)
        {
            deployCanvas.gameObject.SetActive(true);
            deployCanvas.renderMode = RenderMode.WorldSpace;
            deployCanvas.worldCamera = astronautCamera;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void Detect3DUIButtonClick()
    {
        Ray ray = astronautCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5f))
        {
            if (hit.collider.gameObject.name == "Button - Si")
            {
                OnConfirmDeployment();
            }
            else if (hit.collider.gameObject.name == "Button - No")
            {
                CanvasOff();
            }
        }
    }

    public void OnConfirmDeployment()
    {
        if (isDroneDeployed) return;

        if (droneInScene == null || spawnPoint == null)
        {
            Debug.LogError("Faltan asignar las referencias del Dron o el SpawnPoint en Screen_1.");
            return;
        }

        // Mantenemos tu teletransportación estática al presionar "Si"
        droneInScene.transform.position = spawnPoint.position;
        droneInScene.transform.rotation = spawnPoint.rotation;

        if (switcher != null)
        {
            switcher.AllowSwitching();
            switcher.SetControl(true); // Saltamos a la cámara del dron con su HUD
        }

        isDroneDeployed = true;
        CanvasOff();
    }

    public void CanvasOff()
    {
        if (deployCanvas != null) deployCanvas.gameObject.SetActive(false);

        if (!isDroneDeployed)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}