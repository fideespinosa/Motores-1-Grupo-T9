using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviorLVL2 : MonoBehaviour
{
    NavMeshAgent agent;

    [SerializeField] Transform player;
    [SerializeField] Animator animator;

    bool run = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (run)
        {
            agent.destination = player.position;
        }
    }

    public void StartRunning()
    {
        animator.SetBool("StartRun", true);
    }

    public void StartScreaming()
    {
        Debug.Log("grita");
        animator.SetBool("StartScreaming", true);
    }

    public void Run()
    {
        // run = true; habilitar para que empiece a perseguirte
        ActivateObstacles();
    }

    private void ActivateObstacles()
    {

    }
}