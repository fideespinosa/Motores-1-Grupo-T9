using UnityEngine;

public class MinigameSFXController : MonoBehaviour
{
    [Header("UI interna del Minijuego")]
    [SerializeField] private AudioSource interactSource2D;
    [SerializeField] private AudioClip minigameOkClip;
    [SerializeField] private AudioClip minigameErrClip;

    public void PlayFeedback(bool isSuccess)
    {
        if (interactSource2D == null) return;

        AudioClip clipToPlay = isSuccess ? minigameOkClip : minigameErrClip;

        if (clipToPlay != null)
        {
            interactSource2D.PlayOneShot(clipToPlay);
        }
    }
}