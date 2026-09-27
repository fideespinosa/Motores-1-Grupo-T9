using System.Collections;
using UnityEngine;

public class ShipVoiceManager : MonoBehaviour
{
    public static ShipVoiceManager Instance;

    [SerializeField] private AudioSource voiceSource;

    [Header("Tutorial de Inicio")]
    [SerializeField] private AudioClip introTutorialClip;
    [SerializeField] private bool playIntroOnStart = true;
    [SerializeField] private float delayBeforeIntro;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (playIntroOnStart && introTutorialClip != null && voiceSource != null)
        {
            StartCoroutine(PlayIntroWithDelay());
        }
    }

    private IEnumerator PlayIntroWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeIntro);
        PlayVoiceLine(introTutorialClip);
    }

    public void PlayVoiceLine(AudioClip clip)
    {
        if (voiceSource == null || clip == null) return;

        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    public void SkipCurrentVoice()
    {
        if (voiceSource != null && voiceSource.isPlaying)
        {
            voiceSource.Stop();
        }
    }
}