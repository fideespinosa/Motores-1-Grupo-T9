using UnityEngine;

public class SurfaceAudioTrigger : MonoBehaviour
{
    [Header("Configuración de Zona")]
    [SerializeField] private bool setAsHouseZone = true;
    [SerializeField] private bool enableFootstepsScript = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerFootSteps footSteps = other.GetComponent<PlayerFootSteps>();

            if (footSteps != null)
            {
                if (enableFootstepsScript)
                {
                    footSteps.enabled = true;
                }

                footSteps.isInsideHouse = setAsHouseZone;
            }
        }
    }
}