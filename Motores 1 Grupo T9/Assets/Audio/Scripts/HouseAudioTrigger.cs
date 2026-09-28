using UnityEngine;

public class HouseAudioTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (AudioAmbienceController.Instance != null) AudioAmbienceController.Instance.EnterHouseZone();
            if (GameMusicManager.Instance != null) GameMusicManager.Instance.EnterHouseZone();

            
            PlayerFootSteps steps = other.GetComponent<PlayerFootSteps>();
            if (steps != null) steps.isInsideHouse = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (AudioAmbienceController.Instance != null) AudioAmbienceController.Instance.ExitHouseZone();
            if (GameMusicManager.Instance != null) GameMusicManager.Instance.ExitHouseZone();

           
            PlayerFootSteps steps = other.GetComponent<PlayerFootSteps>();
            if (steps != null) steps.isInsideHouse = false;
        }
    }
}