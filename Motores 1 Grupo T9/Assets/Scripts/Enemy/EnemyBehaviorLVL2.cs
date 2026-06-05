using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class EnemyBehaviorLVL2 : MonoBehaviour
{
    NavMeshAgent agent;

    [SerializeField] Transform player;
    [SerializeField] Animator animator;
    [SerializeField] GameObject staticImage;

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
            PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Game Over - Dron");
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
        staticImage.SetActive(true);
         run = true; 
        ActivateObstacles();
    }

    private void ActivateObstacles()
    {

    }
}