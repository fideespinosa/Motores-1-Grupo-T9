using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class MedicationListPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private TextMeshProUGUI listText;

    [Header("Show")]
    [SerializeField] private string showFlag = "medication_task_started";

    [Header("Format")]
    [SerializeField] private string lineFormat = "{0}/{1} de {2}";

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float hideDelay = 1f;

    private bool isShown = false;
    private bool finished = false;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
        }
    }

    private void Start()
    {
        if (MedicationManager.Instance != null)
        {
            MedicationManager.Instance.OnMedicationChanged += Refresh;
        }
    }

    private void OnDestroy()
    {
        if (MedicationManager.Instance != null)
        {
            MedicationManager.Instance.OnMedicationChanged -= Refresh;
        }
    }

    private void Update()
    {
        if (finished) return;
        if (MedicationManager.Instance == null) return;

        if (MedicationManager.Instance.IsComplete())
        {
            finished = true;

            if (isShown)
            {
                StartFade(0f, hideDelay);
            }

            return;
        }

        if (isShown) return;

        string flagToWaitFor = string.IsNullOrEmpty(showFlag) ? "medication_task_started" : showFlag;
        bool flagReady = StoryFlagManager.Instance != null && StoryFlagManager.Instance.HasFlag(flagToWaitFor);

        if (!flagReady) return;

        isShown = true;
        Refresh();
        StartFade(1f, 0f);
    }

    private void StartFade(float targetAlpha, float delay)
    {
        if (panelCanvasGroup == null) return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeTo(targetAlpha, delay));
    }

    private IEnumerator FadeTo(float targetAlpha, float delay)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        float startAlpha = panelCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        panelCanvasGroup.alpha = targetAlpha;
        fadeCoroutine = null;
    }

    private void Refresh()
    {
        if (listText == null || MedicationManager.Instance == null) return;

        StringBuilder builder = new StringBuilder();

        foreach (MedicationRequirement requirement in MedicationManager.Instance.Requirements)
        {
            int collected = MedicationManager.Instance.GetCollected(requirement.medicationId);

            if (builder.Length > 0)
            {
                builder.Append("\n");
            }

            builder.AppendFormat(lineFormat, collected, requirement.requiredAmount, requirement.displayName);
        }

        listText.text = builder.ToString();
    }
}