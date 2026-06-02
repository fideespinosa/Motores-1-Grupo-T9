using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("Lose");
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
         run = true; 
        ActivateObstacles();
    }

    private void ActivateObstacles()
    {

    }
}