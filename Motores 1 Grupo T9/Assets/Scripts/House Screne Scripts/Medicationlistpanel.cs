using System.Text;
using TMPro;
using UnityEngine;

public class MedicationListPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI listText;

    [Header("Show")]
    [SerializeField] private string showFlag = "medication_task_started";

    [Header("Format")]
    [SerializeField] private string lineFormat = "{0}/{1} de {2}";

    private bool isShown = false;

    private void Awake()
    {
        if (panel != null)
        {
            panel.SetActive(false);
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
        if (isShown) return;

        bool flagReady = string.IsNullOrEmpty(showFlag) ||
            (StoryFlagManager.Instance != null && StoryFlagManager.Instance.HasFlag(showFlag));

        if (!flagReady) return;

        isShown = true;

        if (panel != null)
        {
            panel.SetActive(true);
        }

        Refresh();
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