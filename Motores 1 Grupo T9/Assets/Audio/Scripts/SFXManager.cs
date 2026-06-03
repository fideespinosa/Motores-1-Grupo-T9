using UnityEngine;
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }
    public ShipAlarmSFXController Alarm { get; private set; }
    public TransitionSFXController Transition { get; private set; }
    public MinigameSFXController Minigame { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            Alarm = GetComponent<ShipAlarmSFXController>();
            Transition = GetComponent<TransitionSFXController>();
            Minigame = GetComponent<MinigameSFXController>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}