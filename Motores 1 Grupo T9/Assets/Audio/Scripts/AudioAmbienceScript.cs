using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioAmbienceController : MonoBehaviour
{
    public static AudioAmbienceController Instance;

    public enum AmbienceZone { Ship, Cave }

    [Header("Posición Actual")]
    public AmbienceZone currentZone;

    [SerializeField] private Transform shipPlayerTransform;
    [SerializeField] private Transform droneTransform;

    [Header("Beds 2D")]
    [SerializeField] private AudioMixerGroup bedsMixerGroup;
    [SerializeField] private AudioSource bedSourceA;
    [SerializeField] private AudioSource bedSourceB;
    [SerializeField] private AudioClip shipBedClip;
    [SerializeField] private AudioClip caveBedClip;
    [SerializeField] private float crossfadeTime = 3f;

    [Header("Sweets 3D")]
    [SerializeField] private AudioMixerGroup sweetMixerGroup;
    [SerializeField] private AudioClip[] shipSweeteners;
    [SerializeField] private AudioClip[] caveSweeteners;
    [SerializeField] private float minTimeBetweenSweeteners = 8f;
    [SerializeField] private float maxTimeBetweenSweeteners = 20f;
    [SerializeField] private float spawnRadius = 15f;

    [Header("Snapshots de Ambience")]
    [SerializeField] private AudioMixerSnapshot shipSnapshot;
    [SerializeField] private AudioMixerSnapshot caveSnapshot;

    private Coroutine sweetenerRoutine;
    private Coroutine fadeRoutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (bedsMixerGroup != null)
        {
            if (bedSourceA != null) bedSourceA.outputAudioMixerGroup = bedsMixerGroup;
            if (bedSourceB != null) bedSourceB.outputAudioMixerGroup = bedsMixerGroup;
        }
        sweetenerRoutine = StartCoroutine(RoutineSweeteners());
        SwitchZone(currentZone, true);
    }

    public void ToDron()
    {
        Debug.Log("Al Dron");
        SwitchZone(AmbienceZone.Cave, false);
    }

    public void ToShip()
    {
        Debug.Log("A la nave");
        SwitchZone(AmbienceZone.Ship, true);
    }

    private void SwitchZone(AmbienceZone newZone, bool forceInstant = false)
    {
        if (currentZone == newZone && (bedSourceA.isPlaying || bedSourceB.isPlaying) && !forceInstant) return;

        currentZone = newZone;
        AudioClip nuevoClip = (newZone == AmbienceZone.Cave) ? caveBedClip : shipBedClip;

        float tiempoTransicion = forceInstant ? 0.01f : crossfadeTime;
        if (newZone == AmbienceZone.Cave && caveSnapshot != null)
        {
            caveSnapshot.TransitionTo(tiempoTransicion);
        }
        else if (newZone == AmbienceZone.Ship && shipSnapshot != null)
        {
            shipSnapshot.TransitionTo(tiempoTransicion);
        }

        AudioSource fadeOutSource = bedSourceA.isPlaying ? bedSourceA : bedSourceB;
        AudioSource fadeInSource = bedSourceA.isPlaying ? bedSourceB : bedSourceA;

        fadeInSource.clip = nuevoClip;
        fadeInSource.loop = true;
        fadeInSource.Play();

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);

        if (forceInstant)
        {
            fadeInSource.volume = 1f;
            fadeOutSource.volume = 0f;
            fadeOutSource.Stop();
        }
        else
        {
            fadeRoutine = StartCoroutine(Crossfade(fadeOutSource, fadeInSource, crossfadeTime));
        }
    }

    private IEnumerator Crossfade(AudioSource fadeOut, AudioSource fadeIn, float duration)
    {
        float time = 0;
        float maxVolume = 1f;

        float startOutVol = fadeOut.volume;
        float startInVol = fadeIn.volume;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            fadeOut.volume = Mathf.Lerp(startOutVol, 0f, t);
            fadeIn.volume = Mathf.Lerp(startInVol, maxVolume, t);

            yield return null;
        }

        fadeOut.volume = 0f;
        fadeOut.Stop();
    }

    private IEnumerator RoutineSweeteners()
    {
        while (true)
        {
            float waitTime = Random.Range(minTimeBetweenSweeteners, maxTimeBetweenSweeteners);
            yield return new WaitForSeconds(waitTime);

            PlaySweets();
        }
    }

    private void PlaySweets()
    {
        Transform activeTransform;
        AudioClip[] arrayActual;

        if (currentZone == AmbienceZone.Cave)
        {
            activeTransform = droneTransform;
            arrayActual = caveSweeteners;
        }
        else
        {
            activeTransform = shipPlayerTransform;
            arrayActual = shipSweeteners;
        }

        if (activeTransform == null || arrayActual == null || arrayActual.Length == 0) return;

        AudioClip clipSelect = arrayActual[Random.Range(0, arrayActual.Length)];
        if (clipSelect == null) return;

        Vector3 randomPos = activeTransform.position + Random.insideUnitSphere * spawnRadius;

        GameObject tempAudio = new GameObject("Sweetener_" + clipSelect.name);
        tempAudio.transform.position = randomPos;

        AudioSource source = tempAudio.AddComponent<AudioSource>();
        source.clip = clipSelect;

        if (sweetMixerGroup != null)
        {
            source.outputAudioMixerGroup = sweetMixerGroup;
        }

        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = 3f;
        source.maxDistance = 25f;
        source.pitch = Random.Range(0.85f, 1.15f);

        source.Play();

        Destroy(tempAudio, clipSelect.length + 0.1f);
    }
}