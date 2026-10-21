
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalCreditsController : MonoBehaviour
{
    public enum TextAnimation
    {
        Fade,
        Typewriter
    }

    [System.Serializable]
    public class CreditElement
    {
        [Header("Elemento UI")]
        public GameObject target;
        public CanvasGroup canvasGroup;
        public TMP_Text text;
        public Image image;

        [Header("Aparicion")]
        [Min(0f)] public float delay = 0f;
        [Min(0f)] public float fadeInDuration = 1f;

        [Header("Texto")]
        public TextAnimation textAnimation = TextAnimation.Fade;
        [Min(0.001f)] public float characterInterval = 0.04f;

        [Header("Desaparicion")]
        public bool autoHide = true;

        [Min(0f)]
        public float visibleDuration;

        [Min(0f)]
        public float fadeOutDelay = 0f;

        [Min(0f)]
        public float fadeOutDuration = 1f;

        [Header("Movimiento opcional")]
        public bool slideFromBelow = false;
        public float slideDistance = 60f;

        [HideInInspector]
        public Vector2 originalPosition;
    }

    [System.Serializable]
    public class CreditLayer
    {
        [Header("Configuracion de la capa")]
        public string layerName = "Nueva capa";
        public List<CreditElement> elements =
            new List<CreditElement>();

        [Tooltip("Tiempo desde el inicio de la capa hasta la siguiente.")]
        [Min(0f)]
        public float duration = 8f;

        [Tooltip("Evita pasar a otra capa automaticamente.")]
        public bool waitUntilLastLayer = false;
    }

    [Header("Capas en orden")]
    public List<CreditLayer> layers =
        new List<CreditLayer>();

    [Header("Boton permanente")]
    public GameObject continueButton;
    public CanvasGroup buttonCanvasGroup;

    [Min(0f)]
    public float buttonFadeInDuration = 1f;

    [Tooltip("Indice de la capa desde la que aparece el boton. Empieza en 0.")]
    [Min(0)]
    public int buttonStartLayer = 1;

    [Header("Volver al menu")]
    public string menuSceneName = "MainMenu";

    private bool isReturningToMenu;
    private bool buttonFadeStarted;

    private void Start()
    {
        InitializeElements();

        if (continueButton != null)
        {
            continueButton.SetActive(false);
        }

        StartCoroutine(PlayCredits());
    }

    private void InitializeElements()
    {
        foreach (CreditLayer layer in layers)
        {
            foreach (CreditElement element in layer.elements)
            {
                if (element.target == null)
                    continue;

                if (element.canvasGroup == null)
                {
                    element.canvasGroup =
                        element.target.GetComponent<CanvasGroup>();
                }

                if (element.canvasGroup == null)
                {
                    element.canvasGroup =
                        element.target.AddComponent<CanvasGroup>();
                }

                RectTransform rect =
                    element.target.GetComponent<RectTransform>();

                if (rect != null)
                {
                    element.originalPosition = rect.anchoredPosition;
                }

                element.target.SetActive(false);
                element.canvasGroup.alpha = 0f;
                element.canvasGroup.interactable = false;
                element.canvasGroup.blocksRaycasts = false;
            }
        }

        if (continueButton != null)
        {
            if (buttonCanvasGroup == null)
            {
                buttonCanvasGroup =
                    continueButton.GetComponent<CanvasGroup>();
            }

            if (buttonCanvasGroup == null)
            {
                buttonCanvasGroup =
                    continueButton.AddComponent<CanvasGroup>();
            }

            buttonCanvasGroup.alpha = 0f;
            buttonCanvasGroup.interactable = false;
            buttonCanvasGroup.blocksRaycasts = false;
        }
    }


    private IEnumerator PlayCredits()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            CreditLayer layer = layers[i];



            if (i >= buttonStartLayer && continueButton != null)
            {
                if (!buttonFadeStarted)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    buttonFadeStarted = true;
                    continueButton.SetActive(true);
                    StartCoroutine(FadeInButton());
                }
            }

            List<Coroutine> elementCoroutines = new List<Coroutine>();

            foreach (CreditElement element in layer.elements)
            {
                if (element.target != null)
                {
                    Coroutine routine = StartCoroutine(ShowElement(element));
                    elementCoroutines.Add(routine);
                }
            }

            if (layer.waitUntilLastLayer)
                yield break;

            yield return new WaitForSeconds(layer.duration);

            while (true)
            {
                bool anyElementActive = false;

                foreach (CreditElement element in layer.elements)
                {
                    if (element.target != null &&
                        element.target.activeSelf)
                    {
                        anyElementActive = true;
                        break;
                    }
                }

                if (!anyElementActive)
                    break;

                yield return null;
            }
        }
    }


    private IEnumerator ShowElement(CreditElement element)
    {
        if (element.target == null)
            yield break;

        yield return new WaitForSeconds(element.delay);

        if (element.target == null)
            yield break;

        element.target.SetActive(true);

        CanvasGroup group = element.canvasGroup;

        if (group == null)
            yield break;

        group.alpha = 0f;

        RectTransform rect =
            element.target.GetComponent<RectTransform>();

        if (rect != null && element.slideFromBelow)
        {
            rect.anchoredPosition =
                element.originalPosition -
                Vector2.up * element.slideDistance;
        }

        bool useTypewriter =
            element.text != null &&
            element.textAnimation == TextAnimation.Typewriter;

        int totalCharacters = 0;
        float typewriterElapsed = 0f;
        int visibleCharacters = 0;

        if (useTypewriter)
        {
            element.text.maxVisibleCharacters = 0;
            element.text.ForceMeshUpdate();

            totalCharacters =
                element.text.textInfo.characterCount;
        }
        float elapsed = 0f;

        while (elapsed < element.fadeInDuration ||
               (useTypewriter &&
                visibleCharacters < totalCharacters))
        {
            float dt = Time.deltaTime;

            elapsed += dt;
            typewriterElapsed += dt;

            float alpha = element.fadeInDuration <= 0f
                ? 1f
                : Mathf.Clamp01(
                    elapsed / element.fadeInDuration);

            group.alpha = alpha;

            if (rect != null && element.slideFromBelow)
            {
                rect.anchoredPosition = Vector2.Lerp(
                    element.originalPosition -
                    Vector2.up * element.slideDistance,
                    element.originalPosition,
                    Mathf.SmoothStep(0f, 1f, alpha));
            }

            if (useTypewriter)
            {
                float interval =
                    Mathf.Max(0.001f, element.characterInterval);

                while (typewriterElapsed >= interval &&
                       visibleCharacters < totalCharacters)
                {
                    typewriterElapsed -= interval;
                    visibleCharacters++;

                    element.text.maxVisibleCharacters =
                        visibleCharacters;
                }
            }

            yield return null;
        }

        group.alpha = 1f;

        if (useTypewriter)
        {
            element.text.maxVisibleCharacters = totalCharacters;
        }

        if (rect != null && element.slideFromBelow)
        {
            rect.anchoredPosition = element.originalPosition;
        }
        if (!element.autoHide)
            yield break;
        yield return new WaitForSeconds(element.visibleDuration);

        yield return new WaitForSeconds(element.fadeOutDelay);

        yield return StartCoroutine(
            HideElement(element, element.fadeOutDuration));

        if (element.target != null)
        {
            element.target.SetActive(false);
        }
    }

    private IEnumerator HideElement(
        CreditElement element, float duration)
    {
        if (element.canvasGroup == null)
            yield break;

        float startAlpha = element.canvasGroup.alpha;

        if (duration <= 0f)
        {
            element.canvasGroup.alpha = 0f;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            element.canvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                0f,
                Mathf.Clamp01(elapsed / duration));

            yield return null;
        }

        element.canvasGroup.alpha = 0f;
    }

    private IEnumerator FadeInButton()
    {
        if (buttonCanvasGroup == null)
            yield break;

        buttonCanvasGroup.alpha = 0f;
        buttonCanvasGroup.interactable = false;
        buttonCanvasGroup.blocksRaycasts = false;

        float elapsed = 0f;

        if (buttonFadeInDuration <= 0f)
        {
            buttonCanvasGroup.alpha = 1f;
        }
        else
        {
            while (elapsed < buttonFadeInDuration)
            {
                elapsed += Time.deltaTime;

                buttonCanvasGroup.alpha = Mathf.Clamp01(
                    elapsed / buttonFadeInDuration);

                yield return null;
            }
        }

        buttonCanvasGroup.alpha = 1f;
        buttonCanvasGroup.interactable = true;
        buttonCanvasGroup.blocksRaycasts = true;
    }

    public void ReturnToMenu()
    {
        if (isReturningToMenu)
            return;

        isReturningToMenu = true;

        SceneManager.LoadScene(menuSceneName);
    }
}
