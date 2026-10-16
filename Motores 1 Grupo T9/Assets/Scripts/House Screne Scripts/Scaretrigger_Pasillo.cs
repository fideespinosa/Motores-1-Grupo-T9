using System.Collections.Generic;
using UnityEngine;

public class ScareTrigger_Pasillo : MonoBehaviour
{
    [Header("Condiciones")]
    public string requiredKeyFlag = "red_key_picked_up";
    public string flagToSet = "PuertaBano_Cerrada";


    [Header("AudioClip y activación")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip doorSmash;
    [SerializeField] private AudioSource bathNoise;


    [Header("Referencias del Escenario")]
    public GameObject monsterModel;

    [Tooltip("Arrastra aquí todas las puertas que se deben cerrar.")]
    public List<Openable> doorsToClose = new List<Openable>();

    [SerializeField] private AudioSource[] ambientAndMusicSources;
    [SerializeField] private float fadeInDuration;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (StoryFlagManager.Instance == null) return;

        if (!StoryFlagManager.Instance.HasFlag(requiredKeyFlag)) return;

        if (StoryFlagManager.Instance.HasFlag(flagToSet)) return;

        StoryFlagManager.Instance.SetFlag(flagToSet);

        if (monsterModel != null)
        {
            // monsterModel.SetActive(true);
        }

        foreach (Openable door in doorsToClose)
        {
            if (door != null)
            {
                door.Close();
                audioSource.clip = doorSmash;
                audioSource.Play();
                BathNoise();

                

            }
        }

        if (SFXTransitionManager.Instance != null && ambientAndMusicSources.Length > 0)
        {
            SFXTransitionManager.Instance.FadeOutAudios(ambientAndMusicSources, fadeInDuration);
        }

    }

    private void BathNoise()
    {
        bathNoise.enabled = true;
        bathNoise.loop = true;
        bathNoise.Play();
    }

    public void StopBathNoise()
    {
        bathNoise.Stop();
        bathNoise.enabled=false;
    }
}