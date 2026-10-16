using UnityEngine;

public class CatchTrigger : MonoBehaviour
{
    [SerializeField] EnemyBehaviorLVL2 enemy;
    [SerializeField] string playerTag = "Player";

    void Awake()
    {
        if (enemy == null) enemy = GetComponentInParent<EnemyBehaviorLVL2>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            enemy.OnCatchContact();
    }
}