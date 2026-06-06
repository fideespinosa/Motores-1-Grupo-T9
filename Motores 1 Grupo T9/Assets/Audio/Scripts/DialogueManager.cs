using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    
    public static DialogueManager Instance { get; private set; }

    [Header("Reproductor de Voces (2D)")]
    
    [SerializeField] private AudioSource voiceSource;

    [Header("Sistema de Ducking")]
    [SerializeField] private AudioMixerSnapshot normalSnapshot;
    [SerializeField] private AudioMixerSnapshot duckingSnapshot; 
    [SerializeField] private float transitionTime = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

   
    public void PlayVoiceLine(AudioClip clip)
    {
        if (voiceSource == null || clip == null) return;

        
        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();

        
        if (duckingSnapshot != null)
        {
            duckingSnapshot.TransitionTo(transitionTime);
        }

        
        StopAllCoroutines();
        StartCoroutine(RestoreMixAfterAudio(clip.length));
    }

    private IEnumerator RestoreMixAfterAudio(float delay)
    {
        
        yield return new WaitForSeconds(delay);

        
        if (normalSnapshot != null)
        {
            normalSnapshot.TransitionTo(transitionTime);
        }
    }
}