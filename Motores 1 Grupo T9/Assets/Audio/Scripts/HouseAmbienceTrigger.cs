using UnityEngine;

public class HouseAmbienceTrigger : MonoBehaviour
{
   
    [SerializeField] private GameObject ambienceHouse;
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && triggerOnce) return;

       
        if (other.CompareTag("Player"))
        {
            if (ambienceHouse != null)
            {
                ambienceHouse.SetActive(true);
            }

            hasTriggered = true;
        }
    }
}
