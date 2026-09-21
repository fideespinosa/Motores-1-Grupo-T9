using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class TransitionToCave : MonoBehaviour
{
    [Header("Player Detection")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Cinematic")]
    [Tooltip("Animators a disparar al mismo tiempo")]
    [SerializeField] private Animator[] cinematicAnimators;
    [SerializeField] private string playTriggerName = "Play";

    [Header("Scene Change")]
    [SerializeField] private string sceneToLoad;

    private bool triggered = false;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag(playerTag)) return;

        triggered = true;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (cinematicAnimators != null)
        {
            foreach (Animator animator in cinematicAnimators)
            {
                if (animator != null)
                {
                    animator.SetTrigger(playTriggerName);
                }
            }
        }
    }
    
    public void OnCinematicFinished()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}