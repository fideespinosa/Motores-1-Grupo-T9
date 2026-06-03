using UnityEngine;

public class ShipAlarmSFXController : MonoBehaviour
{
    [Header("Configuración de Alarma")]
    [SerializeField] private AudioSource alarmSource3D;
    [SerializeField] private AudioClip alarmClip;

    
    public void SetAlarmState(bool isActive)
    {
        
        if (alarmSource3D == null || alarmClip == null)
        {
            Debug.LogWarning("Falta SFX");
            return;
        }

        if (isActive && !alarmSource3D.isPlaying)
        {
            alarmSource3D.clip = alarmClip;
            alarmSource3D.loop = true;
            alarmSource3D.Play();
        }
        else if (!isActive && alarmSource3D.isPlaying)
        {
            alarmSource3D.Stop();
        }
    }
}