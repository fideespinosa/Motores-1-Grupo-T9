using UnityEngine;

public class Screen_1 : MonoBehaviour
{
    [SerializeField] private ScreensManagerScript screensManager;

    
    [SerializeField] private GameObject deploymentPanel; 

    void Start()
    {
        if (screensManager != null)
        {
            screensManager = screensManager.GetComponent<ScreensManagerScript>();
        }

        if (deploymentPanel != null)
        {
            deploymentPanel.SetActive(false);
        }
    }

    public void ShowDeploymentOptions()
    {
        if (deploymentPanel != null)
        {
            deploymentPanel.SetActive(true); // Hace aparecer el cartel con SÍ y NO

            // Liberamos el mouse para que el jugador pueda interactuar con los botones
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void CanvasOff()
    {
        // Si cierran la pantalla general, por seguridad también apagamos el panel de despliegue
        if (deploymentPanel != null) deploymentPanel.SetActive(false);

        gameObject.SetActive(false);
    }
}