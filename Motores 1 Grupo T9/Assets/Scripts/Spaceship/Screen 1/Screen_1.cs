using UnityEngine;

public class Screen_1 : MonoBehaviour
{
    [SerializeField] private ScreensManagerScript screensManager;

    
    [SerializeField] private Canvas deployCanvas;

 
    [SerializeField] private GameObject droneInScene; 
    [SerializeField] private Transform spawnPoint;     

   
    [SerializeField] private Camera astronautCamera;
    [SerializeField] private GameObject player;

    private PlayerSwitcher switcher;
    private bool isDroneDeployed = false;
    private bool isPlayerLooking = false;

    void Start()
    {
        if (screensManager != null)
        {
            screensManager = screensManager.GetComponent<ScreensManagerScript>();
        }

        switcher = Object.FindFirstObjectByType<PlayerSwitcher>();

        if (astronautCamera == null) astronautCamera = Camera.main;
        if (deployCanvas != null) deployCanvas.gameObject.SetActive(false);

        
        if (droneInScene != null && !isDroneDeployed)
        {
            droneInScene.SetActive(false);
        }
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
            Debug.LogError("Error: Faltan asignar el Dron de la escena o el SpawnPoint en Screen_1.");
            return;
        }

        
        droneInScene.transform.position = spawnPoint.position;
        droneInScene.transform.rotation = spawnPoint.rotation;

        
        droneInScene.SetActive(true);

        
        if (switcher != null)
        {
            switcher.AllowSwitching();
            switcher.SetControl(true); 
        }

        isDroneDeployed = true;
        CanvasOff();
        Debug.Log("Dron en escena despertado y posicionado correctamente.");
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