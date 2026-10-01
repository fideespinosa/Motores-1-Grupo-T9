using UnityEngine;

public class ArenaScript : MonoBehaviour
{
    [SerializeField] PlayerMovement player;
    //[SerializeField] PlayerMovement enemy;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.setSpeed(walkSpeed, runSpeed);
        }

        if (other.CompareTag("EnemyLVL2"))
        {

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.resetSpeed();
        }

        if (other.CompareTag("EnemyLVL2"))
        {

        }
    }
}
