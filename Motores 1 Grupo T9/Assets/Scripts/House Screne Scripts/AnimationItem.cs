using UnityEngine;

public class AnimationItem : MonoBehaviour, IInteractable
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string triggerName = "Play";
    [SerializeField] private bool playOnce = true;

    [Header("Requirement")]
    [SerializeField] private string requiredFlag;

    [Header("Story Flag")]
    [SerializeField] private string flagToSetOnInteract;

    [Header("Outline")]
    [SerializeField] private Outline outline;

    private bool hasPlayed = false;

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
        if (playOnce && hasPlayed) return;

        hasPlayed = true;

        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }

        if (!string.IsNullOrEmpty(flagToSetOnInteract) && StoryFlagManager.Instance != null)
        {
            StoryFlagManager.Instance.SetFlag(flagToSetOnInteract);
        }

        if (playOnce && outline != null)
        {
            outline.enabled = false;
        }
    }

    public void OnHoverEnter()
    {
        if (!IsUnlocked()) return;
        if (playOnce && hasPlayed) return;

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