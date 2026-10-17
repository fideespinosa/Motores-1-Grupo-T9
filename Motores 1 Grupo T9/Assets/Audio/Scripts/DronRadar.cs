using UnityEngine;

public class DroneRadar : MonoBehaviour
{
    [Header("Configuración del Radar")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private LayerMask enemyLayer; 

    [Header("Audio del Pulso")]
    [SerializeField] private AudioSource pingAudioSource;
    [SerializeField] private float pingInterval = 2f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= pingInterval)
        {
            timer = 0f;
            CheckForEnemies();
        }
    }

    private void CheckForEnemies()
    {
        
        bool enemyNearby = Physics.CheckSphere(transform.position, detectionRadius, enemyLayer);

        if (enemyNearby && pingAudioSource != null)
        {
            pingAudioSource.Play();
        }
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}