using UnityEngine;

public class RadioActivator : MonoBehaviour
{
    [SerializeField] private Collider radioCollider;

    private void OnTriggerEnter(Collider Other)
        

    {
        if (Other.CompareTag("Player"))
        {
            if (radioCollider != null)
            {
                radioCollider.enabled = true;
            }

        }
    }
}
