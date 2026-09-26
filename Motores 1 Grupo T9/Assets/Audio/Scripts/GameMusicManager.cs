using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class GameMusicManager : MonoBehaviour
{
    public static GameMusicManager Instance;

    [Header("Mixer y Snapshots")]
    public AudioMixerSnapshot exploreSnapshot;
    public AudioMixerSnapshot combatSnapshot;
    public AudioMixerSnapshot houseSnapshot;
    [SerializeField] private AudioMixer mainMixer;
    public float transitionTime = 2f;

    [Header("Música de Exploración")]
    public AudioSource exploreSource;
    public AudioClip[] exploreClips;
    public float minWaitTime = 30f;
    public float maxWaitTime = 90f;

    [Header("Música de Combate")]
    public AudioSource combatSource;
    public AudioClip combatClip;

    [Header("Música de la Casa")]
    public AudioSource houseSource;
    public AudioClip houseClip;

    private bool isCombatActive = false;
    private bool isInsideHouse = false;

    [Header("Reverb")]
    [SerializeField] private string reverbParamName = "MyExposedParam4";
    [SerializeField] private float normalReverb = 0f;
    [SerializeField] private float combatReverb = -80f;

    private Coroutine reverbCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (exploreSnapshot == null || combatSnapshot == null) return;

        exploreSnapshot.TransitionTo(0.1f);
        StartCoroutine(RutinaExploracionEsporadica());

        if (mainMixer != null)
        {
            mainMixer.SetFloat(reverbParamName, normalReverb);
        }
    }

    private IEnumerator RutinaExploracionEsporadica()
    {
        while (true)
        {
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            if (!isCombatActive && !isInsideHouse && exploreSource != null && !exploreSource.isPlaying && exploreClips != null && exploreClips.Length > 0)
            {
                AudioClip clipAleatorio = exploreClips[Random.Range(0, exploreClips.Length)];
                exploreSource.clip = clipAleatorio;
                exploreSource.Play();
            }
        }
    }

    public void SetCombatState(bool inCombat)
    {
        if (isCombatActive == inCombat) return;
        isCombatActive = inCombat;

        CancelInvoke(nameof(StopCombatMusic));

        if (isCombatActive)
        {
            if (combatSource != null && !combatSource.isPlaying && combatClip != null)
            {
                combatSource.clip = combatClip;
                combatSource.loop = true;
                combatSource.Play();
            }

            if (!isInsideHouse)
            {
                combatSnapshot.TransitionTo(transitionTime);
                if (reverbCoroutine != null) StopCoroutine(reverbCoroutine);
                reverbCoroutine = StartCoroutine(LerpParameter(reverbParamName, combatReverb, transitionTime));
            }
        }
        else
        {
            if (!isInsideHouse)
            {
                exploreSnapshot.TransitionTo(transitionTime);
                Invoke(nameof(StopCombatMusic), transitionTime);
                if (reverbCoroutine != null) StopCoroutine(reverbCoroutine);
                reverbCoroutine = StartCoroutine(LerpParameter(reverbParamName, normalReverb, transitionTime));
            }
            else
            {
                StopCombatMusic();
            }
        }
    }

    public void EnterHouseZone()
    {
        if (isInsideHouse) return;
        isInsideHouse = true;

        if (houseSnapshot != null)
        {
            houseSnapshot.TransitionTo(transitionTime);
        }

        if (exploreSource != null && exploreSource.isPlaying) exploreSource.Pause();
        if (combatSource != null && combatSource.isPlaying) combatSource.Pause();

        if (houseSource != null && houseClip != null)
        {
            houseSource.clip = houseClip;
            houseSource.loop = true;
            houseSource.Play();
        }
    }

    public void ExitHouseZone()
    {
        if (!isInsideHouse) return;
        isInsideHouse = false;

        if (houseSource != null) houseSource.Stop();

        if (isCombatActive)
        {
            if (combatSnapshot != null) combatSnapshot.TransitionTo(transitionTime);
            if (combatSource != null) combatSource.UnPause();
        }
        else
        {
            if (exploreSnapshot != null) exploreSnapshot.TransitionTo(transitionTime);
            if (exploreSource != null) exploreSource.UnPause();
        }
    }

    private IEnumerator LerpParameter(string paramName, float targetValue, float duration)
    {
        if (mainMixer == null) yield break;

        float currentValue;
        mainMixer.GetFloat(paramName, out currentValue);

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float newValue = Mathf.Lerp(currentValue, targetValue, time / duration);
            mainMixer.SetFloat(paramName, newValue);
            yield return null;
        }

        mainMixer.SetFloat(paramName, targetValue);
    }

    private void StopCombatMusic()
    {
        if (combatSource != null)
        {
            combatSource.Stop();
        }
    }
}