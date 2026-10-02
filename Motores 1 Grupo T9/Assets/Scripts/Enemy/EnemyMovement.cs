using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] enemiesAmountOfAttacks amountOfAttacks;
    [Header("Patrulla")]
    public Transform[] waypoints;
    public float moveSpeed = 2.5f;
    public float waypointTolerance = 0.6f;
    public float waitAtWaypoint = 1f;
    [Range(0f, 1f)] public float stoppingDistanceFactor = 0.5f;

    [Header("El FOV")]
    public float detectionRange = 10f;
    public float hearDetectionRange = 4f;

    [Range(0f, 180f)]
    public float fieldOfViewAngle = 90f;
    public LayerMask obstacles;

    [Header("Persecucion / Ataque")]
    public float chaseSpeed = 4f;
    public float proximityChaseRange = 5f;
    public float attackRange = 2.5f;
    public bool patrolDuringMinigame = true;

    [Header("Referencias de Eventos")]
    public PlayerSwitcher switcher;
    public MinigamesManager minigamesManager;
    [SerializeField] InterceptorScript interceptorScript;
    [SerializeField] private Transform spawnPoint;

    [Header("Minijuegos")]
    [SerializeField] UnityEvent[] minigames;
    private int lastMinigame = -1;
    [SerializeField] DroneHitSequence hitSequence;

    // Estado compartido
    private bool playerDead = false;
    private Transform player;
    private NavMeshAgent agent;
    private bool isAttacking = false;
    private bool minigameActive = false;
    private float resetCooldown = 0f;
    private const float resetCooldownTime = 5f;
    private MonsterAudioController audioController;

    // Maquina de estados
    private EnemyStateMachine stateMachine;
    public PatrolState PatrolState { get; private set; }
    public ChaseState ChaseState { get; private set; }
    public AttackState AttackState { get; private set; }

    // Acceso para los estados
    public NavMeshAgent Agent => agent;
    public Transform Player => player;
    public bool IsAttacking => isAttacking;
    public bool MinigameActive => minigameActive;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.updateRotation = false;
            agent.stoppingDistance = waypointTolerance;
        }

        audioController = GetComponent<MonsterAudioController>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;

        if (waypoints == null || waypoints.Length == 0)
            waypoints = new Transform[] { transform };

        if (minigamesManager != null)
        {
            minigamesManager = minigamesManager.GetComponent<MinigamesManager>();
        }

        if (switcher == null)
        {
            switcher = Object.FindFirstObjectByType<PlayerSwitcher>();
        }

        // Estados
        stateMachine = new EnemyStateMachine();
        PatrolState = new PatrolState(this);
        ChaseState = new ChaseState(this);
        AttackState = new AttackState(this);

        stateMachine.ChangeState(PatrolState);
    }

    void Update()
    {
        if (playerDead) { return; }

        if (resetCooldown > 0f)
        {
            resetCooldown -= Time.deltaTime;
        }

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        stateMachine.Tick();
    }

    public void ChangeState(EnemyState next)
    {
        stateMachine.ChangeState(next);
    }

    // ---------- Decision de persecucion (misma condicion que antes) ----------

    public bool ShouldChase()
    {
        if (player == null || minigameActive || resetCooldown > 0f)
            return false;

        if (GameStatusScript.Instance != null && GameStatusScript.Instance.minigameRunning)
            return false;

        return CanSeePlayer()
            || CanHearPlayerNearby()
            || (DistanceToPlayer() <= proximityChaseRange && !isAttacking);
    }

    public float DistanceToPlayer()
    {
        if (player == null) return float.MaxValue;
        return Vector3.Distance(transform.position, player.position);
    }

    // ---------- Eventos de estado (audio / musica) ----------

    public void OnChaseStarted()
    {
        if (audioController != null)
        {
            audioController.PlayRoar();
            GameMusicManager.Instance.SetCombatState(true);
        }
    }

    public void OnChaseEnded()
    {
        if (audioController != null)
        {
            GameMusicManager.Instance.SetCombatState(false);
        }
    }

    public void TriggerAttack()
    {
        if (audioController != null)
            audioController.PlayAttackSound();

        resetCooldown = 15f; //bien hardcodeado

        Debug.Log("Dron interceptado. Iniciando minijuego...");
        Die();
    }

    // ---------- Utilidades compartidas ----------

    public float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f; b.y = 0f;
        return Vector3.Distance(a, b);
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        if (distance > detectionRange)
            return false;

        float angle = Vector3.Angle(transform.forward, toPlayer);

        if (angle > fieldOfViewAngle * 0.5f)
            return false;

        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 direction = (player.position + Vector3.up * 1f - origin).normalized;

        if (Physics.Raycast(origin, direction, distance, obstacles))
        {
            return false;
        }

        return true;
    }

    public bool CanHearPlayerNearby()
    {
        if (player == null)
            return false;

        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= hearDetectionRange;
    }

    public void MoveTowards(Vector3 target)
    {
        MoveTowards(target, moveSpeed);
    }

    public void MoveTowards(Vector3 target, float speed)
    {
        if (agent == null)
            return;

        agent.speed = speed;
        agent.SetDestination(target);

        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            FaceTarget(transform.position + agent.velocity);
        }
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }

    void Die()
    {
        if (playerDead) return;
        Debug.Log("Dron interceptado. Iniciando secuencia de reparacion...");
        TriggerDroneFailure();
    }

    void TriggerDroneFailure()
    {
        if (hitSequence != null && hitSequence.IsPlaying) return;

        amountOfAttacks.setAmount(amountOfAttacks.getAmount() + 1);
        int count = amountOfAttacks.getAmount();
        Debug.Log("ATAQUE nro " + count);

        System.Action startMinigame = () =>
        {
            if (count == 1)
            {
                if (minigamesManager != null) minigamesManager.DronFailure();
            }
            else
            {
                if (interceptorScript != null) interceptorScript.TryStartAppearCycle();
            }
        };

        if (hitSequence != null) hitSequence.Play(startMinigame);
        else startMinigame();
    }

    public void ResetEnemy()
    {
        isAttacking = false;
        minigameActive = false;
        resetCooldown = resetCooldownTime;

        if (agent != null)
        {
            agent.isStopped = false;
            agent.ResetPath();
        }

        stateMachine.ChangeState(PatrolState);
    }

    public void RestartPatrol()
    {
        if (agent != null)
        {
            agent.ResetPath();
            agent.Warp(spawnPoint.position);
        }

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        PatrolState.ResetPatrol();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 left = Quaternion.Euler(0, -fieldOfViewAngle * 0.5f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, fieldOfViewAngle * 0.5f, 0) * transform.forward;

        Gizmos.color = new Color(1f, 0.9f, 0f, 0.4f);
        Gizmos.DrawLine(transform.position, transform.position + left * detectionRange);
        Gizmos.DrawLine(transform.position, transform.position + right * detectionRange);

        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.cyan;

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null) continue;

                Gizmos.DrawSphere(waypoints[i].position, 0.12f);

                int next = (i + 1) % waypoints.Length;

                if (waypoints[next] != null)
                {
                    Gizmos.DrawLine(
                        waypoints[i].position,
                        waypoints[next].position
                    );
                }
            }
        }
    }
}