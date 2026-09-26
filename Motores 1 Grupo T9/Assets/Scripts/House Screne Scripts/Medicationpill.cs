using UnityEngine;

public class MedicationPill : MonoBehaviour, IInteractable
{
    [Header("Medication Id")]
    [Tooltip("Debe coincidir con el Medication Id configurado en el MedicationManager. Cualquier otro valor se trata como pastilla distractora.")]
    [SerializeField] private string medicationId;
    [SerializeField] private string displayName;

    [Header("Outline")]
    [SerializeField] private Outline outline;

    [Header("Name Label")]
    [SerializeField] private WorldSpaceLabel nameLabel;

    [Header("Requirement")]
    [Tooltip("Flag que debe estar activo para poder ver e interactuar con esta pastilla. Dejar vacío si no aplica.")]
    [SerializeField] private string requiredFlag = "medication_task_started";

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clipList; 

    private bool IsUnlocked()
    {
        if (string.IsNullOrEmpty(requiredFlag)) return true;
        return StoryFlagManager.Instance != null && StoryFlagManager.Instance.HasFlag(requiredFlag);
    }

    private void Awake()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    private void PlayRandomPillSound()
    {
        if (clipList.Length == 0) return;

        int indexAlt = Random.Range(0, clipList.Length);

        AudioSource.PlayClipAtPoint(clipList[indexAlt], transform.position);

    }

    public void Action()
    {
        if (!IsUnlocked()) return;
        if (MedicationManager.Instance == null) return;

        bool wasCollected = MedicationManager.Instance.TryCollect(medicationId);

      
        if (wasCollected)
        {
            PlayRandomPillSound();
            gameObject.SetActive(false);
        }
    }

    public void OnHoverEnter()
    {
        if (!IsUnlocked()) return;

        if (outline != null)
        {
            outline.enabled = true;
        }

        if (nameLabel != null)
        {
            nameLabel.Show(displayName);
        }
    }

    public void OnHoverExit()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }

        if (nameLabel != null)
        {
            nameLabel.Hide();
        }
    }
}