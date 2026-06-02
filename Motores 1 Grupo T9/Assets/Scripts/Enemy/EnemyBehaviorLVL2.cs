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
        transform.LookAt(player.transform);
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
        animator.SetBool("StartScreaming", true);
    }

    public void Run()
    {
        //habilitar para que empiece a perseguirte
         run = true; 
        ActivateObstacles();
    }

    private void ActivateObstacles()
    {

    }
}