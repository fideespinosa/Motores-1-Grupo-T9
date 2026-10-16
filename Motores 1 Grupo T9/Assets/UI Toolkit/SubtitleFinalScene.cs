using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubtitleFinalScene : MonoBehaviour
{
    
    [System.Serializable]
    public class SubtitleLines
    {
        [TextArea]
        public string text;
        public Color textColor = Color.white; // Color configurable por línea
        public float sizeText = 80f;
        public float displayDuration = 3f;

        [Header("Posicionamiento Personalizado")]
        [Tooltip("Transform objetivo desde el cual saldrá este texto. Si se deja vacío, mantendrá la posición actual.")]
        public RectTransform targetPosition;

        [Header("Secuencia")]
        [Tooltip("Si se marca, el siguiente subtítulo se mostrará junto con este (en una nueva línea) sin borrar este texto.")]
        public bool showSimultaneouslyWithNext = false;
    }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI displayedText;
    [SerializeField] private GameObject panel;

    [Header("Subtitles Configuration")]
    [SerializeField] private List<SubtitleLines> subtitles;

    [Header("Animation Settings")]
    [Tooltip("Tiempo en segundos entre la aparición de cada letra.")]
    [SerializeField] private float typewriterSpeed = 0.04f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private Coroutine activeSequenceCoroutine;
    private RectTransform textRectTransform;
    private void Awake()
    {
        if (displayedText != null)
        {
            textRectTransform = displayedText.GetComponent<RectTransform>();
        }
    }

    public void Play()
    {
        if (activeSequenceCoroutine != null)
        {
            StopCoroutine(activeSequenceCoroutine);
        }

        displayedText.enabled = true;
        activeSequenceCoroutine = StartCoroutine(PlaySequence());
    }

    public void Stop()
    {
        if (activeSequenceCoroutine != null)
        {
            StopCoroutine(activeSequenceCoroutine);
            activeSequenceCoroutine = null;
        }

        displayedText.alpha = 1f;
        displayedText.text = "";
        displayedText.enabled = false;

        Debug.Log("Secuencia de subtítulos detenida.");
    }

    private IEnumerator PlaySequence()
    {
        // Activar y mostrar el panel con Fade In inicial (sin Fade Out al final)
        if (panel != null)
        {
            panel.SetActive(true);
        }

        for (int i = 0; i < subtitles.Count; i++)
        {
            SubtitleLines currentLine = subtitles[i];

            // 1. Cambiar posición si se asignó un targetPosition específico
            if (currentLine.targetPosition != null && textRectTransform != null)
            {
                textRectTransform.position = currentLine.targetPosition.position;
            }

            // 2. Formatear el texto con su color usando etiquetas RichText HTML de TMP
            string hexColor = ColorUtility.ToHtmlStringRGBA(currentLine.textColor);
            string formattedLineText = $"<size={currentLine.sizeText}><color=#{hexColor}>{currentLine.text}</color></size>";

            // Si es un subtítulo acumulativo, añadimos salto de línea al texto existente
            int baseCharacterCount = displayedText.textInfo != null ? displayedText.textInfo.characterCount : 0;

            if (string.IsNullOrEmpty(displayedText.text))
            {
                displayedText.text = formattedLineText;
            }
            else
            {
                displayedText.text += "\n" + formattedLineText;
            }

            // 3. Animación letra por letra del nuevo fragmento
            yield return TypewriterTextSegment(baseCharacterCount);

            // 4. Esperar la duración de pantalla asignada
            yield return new WaitForSeconds(currentLine.displayDuration);

            // 5. Verificar si debe salir el siguiente inmediatamente o desvanecer
            if (!currentLine.showSimultaneouslyWithNext)
            {
                // Si no se acumula con el siguiente, desvanecemos y limpiamos para la próxima frase
                if (i < subtitles.Count - 1)
                {
                    yield return FadeText(1f, 0f, fadeOutDuration);
                    displayedText.text = "";
                    displayedText.alpha = 1f;
                }
            }
        }

        Debug.Log("Terminó la secuencia de texto.");
        //displayedText.text = "";
        //displayedText.enabled = false;
        activeSequenceCoroutine = null;
    }
    /// <summary>
    /// Animación letra por letra que continúa desde la cantidad de caracteres anteriores.
    /// </summary>
    private IEnumerator TypewriterTextSegment(int startIndex)
    {
        displayedText.ForceMeshUpdate();
        int totalVisibleCharacters = displayedText.textInfo.characterCount;

        for (int c = startIndex; c <= totalVisibleCharacters; c++)
        {
            displayedText.maxVisibleCharacters = c;
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }

    private IEnumerator FadeText(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            displayedText.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        displayedText.alpha = to;
    }
}

