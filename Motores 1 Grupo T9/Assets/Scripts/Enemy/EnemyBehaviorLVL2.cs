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
    [SerializeField] GameObject panelFadeScript;

    [Header("Animaciones")]
    [SerializeField] private float delayAfterScream = 0.5f;

    [Header("Intro")]
    [SerializeField] private Transform introTarget;            // hacia dónde camina durante el Scream
    [SerializeField] private float introMoveSpeed = 2f;        // velocidad del avance
    [SerializeField] private float introArriveDistance = 0.3f; // distancia a la que se considera "llegó"
    private float runSpeed;
    private bool introMoving = false;

    [Header("Viewpoint")]
    [SerializeField] private Transform viewPoint;

    [Header("Jumpscare / Muerte")]
    [SerializeField] private GameObject playerObject;
    private MonoBehaviour playerMovementScript;
    private Animator playerDeathAnimator;

    [Header("Cámara")]
    [SerializeField] private CameraSequenceController cameraController;
    [SerializeField] private float cameraLookSpeed = 5f;
    [SerializeField] FlashlightFlicker flashlightFlicker;

    bool run = false;
    private bool jumpscareActive = false;
    private bool introSequenceActive = false;
    private bool contactEnabled = false; // bloquea el trigger hasta que termine el intro

    public bool IntroDone { get; private set; }
    public bool ScreamEnded { get; private set; }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioController = GetComponent<MonsterAudioController>();
        runSpeed = agent.speed;   // velocidad de correr = la del NavMeshAgent en el inspector
        agent.enabled = false;    // el agente se activa recién al empezar a correr

        if (playerObject != null)
        {
            playerMovementScript = playerObject.GetComponent<PlayerMovement>();
            playerDeathAnimator = playerObject.GetComponent<Animator>();
        }
    }

    void Update()
    {
        // Intro: se mueve por script (sin NavMesh) hacia introTarget
        if (introMoving && introTarget != null)
        {
            Vector3 dir = introTarget.position - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(dir);

            transform.position = Vector3.MoveTowards(
                transform.position,
                introTarget.position,
                introMoveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, introTarget.position) <= introArriveDistance)
            {
                introMoving = false;
                Debug.Log($"[{Time.time:F2}] llegó al introTarget");
            }
        }

        // Persecución: con NavMeshAgent
        if (run)
        {
            Vector3 flat = player.position - transform.position;
            flat.y = 0f;
            if (flat.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(flat);

            agent.destination = player.position;
        }
    }

    // ---------- FASE 1: Spawn / intro ----------

    public void StartScreaming()
    {
        Debug.Log("StartScreaming llamado");
        introSequenceActive = true;
        animator.SetTrigger("StartIntro"); // Idle -> Scream

        if (introTarget != null)
            introMoving = true;

        if (GameMusicManager.Instance != null)
            GameMusicManager.Instance.SetCombatState(true);
    }

    // Animation Event en el frame del grito
    public void PlayScreamRoar()
    {
        if (audioController != null)
            audioController.PlayRoar();
    }

    // Animation Event al final del clip Scream
    public void OnScreamEnd()
    {
        Debug.Log($"[{Time.time:F2}] OnScreamEnd");
        if (jumpscareActive)
            OnDeathScreamEnd();
        else if (introSequenceActive)
        {
            ScreamEnded = true; // el jugador se libera acá
            StartCoroutine(IntroToRunRoutine());
        }
    }

    private IEnumerator IntroToRunRoutine()
    {
        introSequenceActive = false;

        // si el clip terminó antes de que llegue al punto, espera a que llegue
        yield return new WaitUntil(() => !introMoving);

        yield return new WaitForSeconds(delayAfterScream);

        animator.SetBool("IsRunning", true); // Run

        // panel que activa shift para correr!!
        if (panelFadeScript != null)
            panelFadeScript.SetActive(true);

        if (flashlightFlicker != null)
            flashlightFlicker.StartFlicker();

        Debug.Log($"[{Time.time:F2}] IntroDone");
        IntroDone = true;
        Run();

        contactEnabled = true; // recién acá el contacto puede matarte
    }

    public void Run()
    {
        Debug.Log($"[{Time.time:F2}] Run llamado");

        if (staticImage != null)
            staticImage.SetActive(true);

        NavMeshHit hit;
        if (!NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            Debug.LogWarning("Run: no hay NavMesh a menos de 5 m del enemigo");
            return;
        }

        transform.position = hit.position; // lo apoya sobre el NavMesh
        agent.enabled = true;              // recién ahora se enciende el agente
        agent.speed = runSpeed;
        agent.isStopped = false;
        run = true;
    }

    // ---------- FASE 2: Contacto / muerte ----------

    private void OnTriggerEnter(Collider other)
    {
        if (!contactEnabled) return; // ignora cualquier contacto antes de tiempo

        if (other.gameObject.CompareTag("Player") && !jumpscareActive)
        {
            SceneManager.LoadScene("Gracias por jugar escena"); // borrar linea cuando se implemente animacion del enemigo que te atrapa
            jumpscareActive = true;
            StartDeathSequence();
        }
    }

    private void StartDeathSequence()
    {
        Debug.Log("StartDeathSequence llamado");
        run = false;
        introMoving = false;

        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        animator.applyRootMotion = false;
        animator.SetBool("IsRunning", false); // vuelve al Scream

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (cameraController != null && viewPoint != null)
            cameraController.LookAtViewpoint(viewPoint, cameraLookSpeed);

        if (playerDeathAnimator != null)
            playerDeathAnimator.SetTrigger("play");
    }

    // Se llama desde OnScreamEnd cuando el Scream es el de la muerte
    public void OnDeathScreamEnd()
    {
        Debug.Log("OnDeathScreamEnd llamado");

        if (cameraController != null)
            cameraController.StopLook();

        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Gracias por jugar escena");
    }
}