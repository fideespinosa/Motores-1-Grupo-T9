using UnityEngine;

public class EnemySpawnLater : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.SetActive(true);
            Destroy(gameObject);
        }
    }
}