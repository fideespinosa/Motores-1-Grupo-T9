using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class RadioAudioController : MonoBehaviour
{
    [Header("Fuente de intereferencia")]
    [SerializeField] private AudioSource radioInterference;

    [Header("Fuente de spots")]
    [SerializeField] private AudioSource radioSpots;
    [SerializeField] private AudioClip[] spotClips;
    [Header("Minimos y maximos entre clips")]
    [SerializeField] private float minTimeSpots;
    [SerializeField] private float maxTimeSpots;


    void Start()
    {
        if (radioInterference != null && !radioInterference.isPlaying)
        {
            radioInterference.loop = true;
            radioInterference.Play();

        }

        if (spotClips.Length>0 && radioSpots != null)
        {
            StartCoroutine(PlaybackRoutine());
        }
    }

    private IEnumerator PlaybackRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minTimeSpots, maxTimeSpots);
            yield return new WaitForSeconds(waitTime);

            int altIndex = Random.Range(0, spotClips.Length);
            AudioClip actualSpot = spotClips[altIndex];

            radioSpots.clip = actualSpot;
            radioSpots.Play();

            yield return new WaitForSeconds(actualSpot.length);
        }
    }
   
}
