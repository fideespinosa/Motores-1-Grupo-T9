using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class TransitionToCave : MonoBehaviour
{
    [SerializeField] GameObject HUD;
    [Header("Player Detection")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Cinematic")]
    [SerializeField] private Animator cameraAnimator;
    [SerializeField] private Animator eyelidsAnimator;
    [SerializeField] private string playTriggerName = "Play";

    [Header("Scene Change")]
    [SerializeField] private string sceneToLoad;

    private bool triggered = false;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    public void StartAnimation()
    {
        if (triggered) return;
        triggered = true;

        HUD.SetActive(false);

        if (playerMovement != null)
        {
            Debug.Log("asd");
            playerMovement.enabled = false;
        }

        if (cameraAnimator != null)
        {
            cameraAnimator.SetTrigger(playTriggerName);
        }
    }

    public void TriggerEyesClose()
    {
        if (eyelidsAnimator != null)
        {
            Debug.Log("aaaa");
            eyelidsAnimator.SetTrigger(playTriggerName);
        }
    }

    public void OnCinematicFinished()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}