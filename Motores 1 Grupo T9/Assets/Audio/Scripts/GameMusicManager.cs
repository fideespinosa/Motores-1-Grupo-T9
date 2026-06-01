using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class GameMusicManager : MonoBehaviour
{
    public static GameMusicManager Instance;

    [Header("Mixer y Snapshots")]
    public AudioMixerSnapshot exploreSnapshot;
    public AudioMixerSnapshot combatSnapshot;
    public float transitionTime = 2f; 

    [Header("Música de Exploración")]
    public AudioSource exploreSource;
    public AudioClip[] exploreClips;
    public float minWaitTime = 30f;  
    public float maxWaitTime = 90f;  

    [Header("Música de Combate")]
    public AudioSource combatSource;
    public AudioClip combatClip;

    private bool isCombatActive = false;

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
    }

    private IEnumerator RutinaExploracionEsporadica()
    {
        while (true)
        {
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            if (!isCombatActive && exploreSource != null && !exploreSource.isPlaying && exploreClips != null && exploreClips.Length > 0)
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

            combatSnapshot.TransitionTo(transitionTime);
        }
        else
        {
            exploreSnapshot.TransitionTo(transitionTime);
            Invoke(nameof(StopCombatMusic), transitionTime);
        }
    }

    private void StopCombatMusic()
    {
        if (combatSource != null)
        {
            combatSource.Stop();
        }
    }
}