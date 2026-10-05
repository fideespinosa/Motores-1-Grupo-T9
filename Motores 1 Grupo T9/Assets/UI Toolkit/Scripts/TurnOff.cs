using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class TurnOff : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement transitionOverlay;

    [Header("Material & Shader Graph")]
    [SerializeField] private Material tvShaderMaterial;
    [SerializeField] private float transitionDuration = 0.5f;

    private Coroutine currentCoroutine;

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        transitionOverlay = root.Q<VisualElement>("TurnOffVE");

        transitionOverlay.style.display = DisplayStyle.None;
    }
    /// <summary>
    /// Método público para disparar el efecto.
    /// enable = true (inicia el apagado), enable = false (inicia el encendido/reapertura)
    /// </summary>
    public void TriggerOverlay(bool enable)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(AnimateOverlay(enable));
    }

    private IEnumerator AnimateOverlay(bool isTurningOff)
    {
        Debug.Log("entre a la corretina");
        if (transitionOverlay == null || tvShaderMaterial == null)
            yield break;

        // Mostrar siempre el overlay mientras la animación esté activa
        transitionOverlay.style.display = DisplayStyle.Flex;

        float elapsed = 0f;
        float startValue = isTurningOff ? 0f : 1f;
        float targetValue = isTurningOff ? 1f : 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float progress = Mathf.Lerp(startValue, targetValue, t);

            tvShaderMaterial.SetFloat("_Progress", progress);

            yield return null;
        }

        tvShaderMaterial.SetFloat("_Progress", targetValue);

        // Si ya terminó de encenderse (reapertura), ocultamos el overlay para permitir clics
        if (!isTurningOff)
        {
            transitionOverlay.style.display = DisplayStyle.None;
        }

        currentCoroutine = null;
    }
}
