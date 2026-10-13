using Unity.VisualScripting;
using UnityEngine;

public class SoundItem : MonoBehaviour, IInteractable
{
    [Header("Requirement")]
    [SerializeField] private string requiredFlag;

    [Header("Outline")]
    [SerializeField] private Outline outline;

    [Header("Acordes de Guitarra")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] sfxSequence;
    private int currentClickIndex = 0;

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

    public void Action()
    {
        if (!IsUnlocked()) return;

        Debug.Log("prueba");
        if (audioSource != null && sfxSequence != null && sfxSequence.Length > 0)
        {
            audioSource.PlayOneShot(sfxSequence[currentClickIndex]);

            currentClickIndex = (currentClickIndex + 1) % sfxSequence.Length;
        }
    }

    public void OnHoverEnter()
    {
        if (!IsUnlocked()) return;

        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void OnHoverExit()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}