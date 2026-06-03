using UnityEngine;
using UnityEngine.UI;

public class DroneDeploymentManager : MonoBehaviour
{
    
    [SerializeField] private GameObject dronePrefab; 
    [SerializeField] private Transform spawnPoint;   

   
    [SerializeField] private GameObject deploymentPanel;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private PlayerSwitcher switcher;
    private bool isDroneDeployed = false;

    void Start()
    {
        switcher = Object.FindFirstObjectByType<PlayerSwitcher>();

        // Configuramos los botones por código para evitar errores en el Inspector
        if (yesButton != null) yesButton.onClick.AddListener(DeployDrone);
        if (noButton != null) noButton.onClick.AddListener(ClosePanel);

        // El panel empieza oculto
        if (deploymentPanel != null) deploymentPanel.SetActive(false);
    }

    // Método público para cuando el astronauta interactúa con la terminal
    public void OpenDeploymentPanel()
    {
        if (isDroneDeployed)
        {
            Debug.Log("El dron ya se encuentra desplegado en el terreno.");
            return;
        }

        deploymentPanel.SetActive(true);

        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void DeployDrone()
    {
        if (dronePrefab == null || spawnPoint == null)
        {
            Debug.LogError("Faltan asignar referencias en el DroneDeploymentManager.");
            return;
        }

       
        GameObject spawnedDrone = Instantiate(dronePrefab, spawnPoint.position, spawnPoint.rotation);

        
        if (switcher != null)
        {
            DroneController controller = spawnedDrone.GetComponent<DroneController>();

            
            switcher.droneMovement = controller;

            
            switcher.droneCameraControl = controller;
            switcher.droneCamera = spawnedDrone.GetComponentInChildren<Camera>();

            
            switcher.AllowSwitching();

            
        }

        isDroneDeployed = true;
        ClosePanel();
        Debug.Log("Dron de exploración desplegado con éxito en el punto de lanzamiento.");
    }

    private void ClosePanel()
    {
        deploymentPanel.SetActive(false);

        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}