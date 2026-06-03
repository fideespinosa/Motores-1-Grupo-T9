using UnityEngine;
using UnityEngine.Audio;

public class MenuMusicManager : MonoBehaviour
{

    [Header("Musica para pantallas/menus")]

    [SerializeField] private AudioClip musicTrack;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixerGroup musicMixerGroup;


    void Start()
    {
        if (audioSource != null && musicTrack != null)
        {
            audioSource.clip = musicTrack;
            audioSource.loop = true;
            audioSource.Play();

        }
    }

}