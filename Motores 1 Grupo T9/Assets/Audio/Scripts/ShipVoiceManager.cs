using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ShipVoiceManager : MonoBehaviour
{
    public static ShipVoiceManager Instance;

    [SerializeField] private AudioSource voiceSource;

    [Header("Tutorial de Inicio")]
    [SerializeField] private AudioClip introTutorialClip;
    [SerializeField] private bool playIntroOnStart = true;
    [SerializeField] private float delayBeforeIntro;

    [Header("IA de minijuego")]
    [SerializeField] private AudioClip alarmIAClip;
    [SerializeField] private AudioClip minigameOkClip;


    [Header("IA  de amenazas")]

    [SerializeField] private AudioClip firstThreat;
    [SerializeField] private AudioClip pulseThreat;



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

    public void AlarmOn()
    {
        if (voiceSource != null)
        {
            voiceSource.clip = alarmIAClip;
            voiceSource.Play();
            Debug.Log("Audio de Dron desactivado");
        }
    }

    public void AlarmSolved()
    {
        if (voiceSource != null)
        {
            voiceSource.clip = minigameOkClip;
            voiceSource.Play();
            Debug.Log("audio de alarma resuelta");
        }
    }

    public void FisrtEnemy()
    {
        if (voiceSource != null)
        {
            voiceSource.clip = firstThreat;
            voiceSource.Play();
            Debug.Log("Primer enemigo sonando");
        }
    }

    public void PulseEnemy()
    {
        if (voiceSource != null)
        {
            voiceSource.clip = pulseThreat;
            voiceSource.Play();
            Debug.Log("pulso de amenazas");
        }
    }

}