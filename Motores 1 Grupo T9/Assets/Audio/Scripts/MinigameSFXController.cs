using UnityEngine;

public class MinigameSFXController : MonoBehaviour
{
    [Header("UI interna del Minijuego")]
    [SerializeField] private AudioSource interactSource2D;
    [SerializeField] private AudioClip minigameOkClip;
    [SerializeField] private AudioClip minigameErrClip;

    [SerializeField] private AudioSource timerSource;
    [SerializeField] private AudioClip timerSound;

    public void PlayFeedback(bool isSuccess)
    {
        if (interactSource2D == null) return;

        AudioClip clipToPlay = isSuccess ? minigameOkClip : minigameErrClip;

        if (clipToPlay != null)
        {
            interactSource2D.PlayOneShot(clipToPlay);
        }
    }

    public void TimerSFXOn()
    {
        if (timerSource != null)
        {
            timerSource.clip = timerSound;
            timerSource.Play();
        }
    }

    public void TimerOK()
    {
        if (timerSource != null && timerSource.isPlaying)
        {
            timerSource.Stop();

        }
    }
}