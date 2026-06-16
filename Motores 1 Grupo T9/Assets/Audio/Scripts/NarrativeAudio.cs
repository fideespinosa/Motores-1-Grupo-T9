using UnityEngine;
using UnityEngine.Audio;

public class NarrativeItem : MonoBehaviour
{
    [Header("Audio del Ítem")]

    public AudioClip voiceClip;
    [SerializeField] AudioMixerGroup dialogueBus;

    
    private bool wasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        
        if (!wasTriggered && other.CompareTag("Player"))
        {
            wasTriggered = true;

            
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.PlayVoiceLine(voiceClip);
            }
        }
    }
}