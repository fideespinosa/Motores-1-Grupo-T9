using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class CRTGlitchTransitionController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement glitchOverlay;
    [SerializeField] private Material glitchMaterial;
    private Coroutine glitchRoutine;

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogError("¡ATENCIÓN! No hay ningún UIDocument asignado en CRTGlitchTransitionController.");
            return;
        }

        var root = uiDocument.rootVisualElement;
        if (root != null)
        {
            // Intenta buscar el elemento con el nombre exacto que le pusiste en el UXML
            glitchOverlay = root.Q<VisualElement>("GlitchOverlay");

            if (glitchOverlay == null)
            {
                Debug.LogError("¡ATENCIÓN! No se encontró ningún VisualElement llamado 'GlitchOverlay' en el UXML.");
            }
            else
            {
                Debug.Log("GlitchOverlay encontrado con éxito en el UI Document.");
                glitchOverlay.style.display = DisplayStyle.None;
                glitchOverlay.style.opacity = 0f;
            }
        }
    }

    public void TriggerGlitchTransition(System.Action onPeakTransition)
    {
        if (glitchRoutine != null) StopCoroutine(glitchRoutine);
        glitchRoutine = StartCoroutine(RoutineGlitch(onPeakTransition));
    }

    private IEnumerator RoutineGlitch(System.Action onPeak)
    {
        if (glitchOverlay == null) yield break;

        // 1. Mostrar el overlay
        glitchOverlay.style.display = DisplayStyle.Flex;

        // 2. Efecto de parpadeo de estática analógica (Flicker) como en el video
        float timer = 0f;
        float totalDuration = 0.5f;

        while (timer < totalDuration)
        {
            timer += Time.deltaTime;

            // Alterna la opacidad rápidamente entre valores aleatorios para crear parpadeo
            float randomOpacity = Random.Range(0.4f, 1.0f);
            glitchOverlay.style.opacity = randomOpacity;

            // Si el shader tiene una propiedad _GlitchIntensity, la variamos dinámicamente
            if (glitchMaterial != null)
            {
                glitchMaterial.SetFloat("_GlitchIntensity", Random.Range(0.2f, 1.0f));
            }

            yield return new WaitForSeconds(0.03f); // Controla la velocidad de estática
        }

        // Opacidad máxima en el punto medio de la transición
        glitchOverlay.style.opacity = 1f;

        // 3. Ejecutar la acción del evento (cambio de cámara, escena o interfaz)
        onPeak?.Invoke();

        yield return new WaitForSeconds(0.1f);

        // 4. Apagar progresivamente el ruido
        timer = 0f;
        float fadeOutDuration = 0.2f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            glitchOverlay.style.opacity = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
            yield return null;
        }

        // Ocultar al finalizar
        glitchOverlay.style.opacity = 0f;
        glitchOverlay.style.display = DisplayStyle.None;
    }
}
