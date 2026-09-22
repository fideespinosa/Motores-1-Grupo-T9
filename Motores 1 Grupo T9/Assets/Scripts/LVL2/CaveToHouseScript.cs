using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CaveToHouseScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject Panel;
    [SerializeField] DroneController droneController;
    [SerializeField] PlayerSwitcher playerSwitcher;

    [Header("Transition")]
    [SerializeField] string SceneName;
    [SerializeField] float FadeDuration = 4f;

    private Image panelImage;
    private bool transitioning = false;

    void Start()
    {
        panelImage = Panel.GetComponent<Image>();

        // Empieza completamente transparente
        Color color = panelImage.color;
        color.a = 0f;
        panelImage.color = color;

        Panel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !transitioning)
        {
            StartTransition();
        }
    }

    public void StartTransition()
    {
        if (transitioning)
            return;

        transitioning = true;

        // Desactivar controles
        droneController.enabled = false;
        playerSwitcher.enabled = false;

        // Activar el panel blanco
        Panel.SetActive(true);

        StartCoroutine(FadeToWhite());
    }

    IEnumerator FadeToWhite()
    {
        float elapsed = 0f;

        Color color = panelImage.color;
        color.a = 0f;

        while (elapsed < FadeDuration)
        {
            elapsed += Time.deltaTime;

            // Aumenta progresivamente el alfa del blanco
            float alpha = Mathf.Lerp(0f, 1f, elapsed / FadeDuration);

            color.a = alpha;
            panelImage.color = color;

            yield return null;
        }

        // Asegurar blanco completamente opaco
        color.a = 1f;
        panelImage.color = color;

        // Cambiar a la escena del recuerdo
        SceneManager.LoadScene(SceneName);
    }
}