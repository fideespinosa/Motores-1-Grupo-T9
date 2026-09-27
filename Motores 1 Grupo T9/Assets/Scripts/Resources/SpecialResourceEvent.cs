using UnityEngine;
using TMPro;
public class SpecialResourceEvent : MonoBehaviour
{
    [SerializeField] GameObject enemy;

    bool isTaken = false;
    // [SerializeField] GameObject particles;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isTaken = true;
            gameObject.SetActive(false);
            enemy.SetActive(true);

            EnemyBehaviorLVL2 enemyScript = enemy.GetComponent<EnemyBehaviorLVL2>();
            if (enemyScript != null)
            {
                enemyScript.StartScreaming();
            }
        }
    }

    public bool GetStatus()
    {
        return isTaken;
    }
}