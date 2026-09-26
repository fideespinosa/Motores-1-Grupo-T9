using UnityEngine;

public class HouseAudioTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (AudioAmbienceController.Instance != null)
            {
                AudioAmbienceController.Instance.EnterHouseZone();
            }

            if (GameMusicManager.Instance != null)
            {
                GameMusicManager.Instance.EnterHouseZone();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (AudioAmbienceController.Instance != null)
            {
                AudioAmbienceController.Instance.ExitHouseZone();
            }

            if (GameMusicManager.Instance != null)
            {
                GameMusicManager.Instance.ExitHouseZone();
            }
        }
    }
}