using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class EnemyBehaviorLVL2 : MonoBehaviour
{
    NavMeshAgent agent;

    [Header("Referencias base")]
    [SerializeField] Transform player;
    [SerializeField] Animator animator;
    [SerializeField] GameObject staticImage;
    [SerializeField] MonsterAudioController audioController;

    [Header("Viewpoint")]
    [SerializeField] private Transform viewPoint;

    [Header("Jumpscare / Muerte")]
    [SerializeField] private GameObject playerObject;
    private MonoBehaviour playerMovementScript;
    private Animator playerDeathAnimator;

    [Header("Cámara")]
    [SerializeField] private CameraSequenceController cameraController;
    [SerializeField] private float cameraMoveDuration = 1f;
    [SerializeField] private float cameraReturnDuration = 1f;
    [SerializeField] private float cameraLookSpeed = 5f;

    bool run = false;
    private bool jumpscareActive = false;
    private bool introSequenceActive = false;
    private bool contactEnabled = false; // NUEVO: bloquea el trigger hasta que termine el intro

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioController = GetComponent<MonsterAudioController>();
        agent.isStopped = true;

        if (playerObject != null)
        {
            playerMovementScript = playerObject.GetComponent<PlayerMovement>();
            playerDeathAnimator = playerObject.GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (run)
        {
            transform.LookAt(player.transform);
            agent.destination = player.position;
        }
    }

    // ---------- FASE 1: Spawn / grito inicial ----------

    public void StartScreaming()
    {
        introSequenceActive = true;

        animator.SetBool("StartScreaming", true);

        if (audioController != null)
        {
            audioController.PlayRoar();
        }

        if (GameMusicManager.Instance != null)
        {
            GameMusicManager.Instance.SetCombatState(true);
        }

        if (cameraController != null && viewPoint != null)
        {
            cameraController.MoveToViewpoint(viewPoint, cameraMoveDuration);
        }
    }

    public void StartRunning()
    {
        animator.SetBool("StartRun", true);
    }

    // Llamado por Animation Event al final del clip de grito inicial
    public void OnIntroScreamEnd()
    {
        Debug.Log("OnIntroScreamEnd llamado");
        Debug.Log("OnIntroScreamEnd llamado - introSequenceActive: " + introSequenceActive);
        if (!introSequenceActive) return;
        introSequenceActive = false;

        animator.SetBool("StartScreaming", false);

        if (cameraController != null)
        {
            cameraController.ReturnToPlayer(cameraReturnDuration);
        }

        Run();

        contactEnabled = true; // NUEVO: recién acá el contacto puede matarte
    }

    public void Run()
    {
        staticImage.SetActive(true);
        run = true;
        agent.isStopped = false;
    }

    // ---------- FASE 2: Contacto / muerte ----------

    private void OnTriggerEnter(Collider other)
    {
        if (!contactEnabled) return; // NUEVO: ignora cualquier contacto antes de tiempo

        if (other.gameObject.CompareTag("Player") && !jumpscareActive)
        {
            jumpscareActive = true;
            StartDeathSequence();
        }
    }

    private void StartDeathSequence()
    {
        Debug.Log("StartDeathSequence llamado");
        run = false;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.updatePosition = false;
        agent.updateRotation = false;

        animator.SetBool("StartRun", false);
        animator.applyRootMotion = false;
        animator.SetTrigger("DeathScream");

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        if (audioController != null)
        {
            audioController.PlayRoar();
        }

        if (cameraController != null && viewPoint != null)
        {
            cameraController.LookAtViewpoint(viewPoint, cameraLookSpeed);
        }

        if (playerDeathAnimator != null)
        {
            playerDeathAnimator.SetTrigger("play");
        }
    }

    // Llamado por Animation Event al final del clip de grito de muerte
    public void OnDeathScreamEnd()
    {
        Debug.Log("OnDeathScreamEnd llamado");

        if (cameraController != null)
        {
            cameraController.StopLook();
        }

        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Game Over - Dron");
    }
}