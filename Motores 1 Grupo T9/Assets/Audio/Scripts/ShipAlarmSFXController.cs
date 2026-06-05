using UnityEngine;
using UnityEngine.Audio;

public class ShipAlarmSFXController : MonoBehaviour
{
    [Header("Configuración de Alarma")]
    [SerializeField] private AudioSource alarmSource3D;
    [SerializeField] private AudioClip alarmClip;

    [SerializeField] private AudioMixerSnapshot normalSnapshot;

   
    [SerializeField] private AudioMixerSnapshot alarmSnapshot;

    [SerializeField] private float transitionTime = 1.5f;

    public void SetAlarmState(bool isActive)
    {
        if (isActive)
        {
            
            if (alarmSource3D != null && alarmClip != null)
            {
                alarmSource3D.clip = alarmClip;
                alarmSource3D.loop = true;
                if (!alarmSource3D.isPlaying) alarmSource3D.Play();
            }

            
            if (alarmSnapshot != null)
            {
                alarmSnapshot.TransitionTo(transitionTime);
                Debug.Log("Audio: Snapshot de Alarma Activado");
            }
        }
        else
        {
            
            if (alarmSource3D != null)
            {
                alarmSource3D.Stop();
            }

            
            if (normalSnapshot != null)
            {
                normalSnapshot.TransitionTo(transitionTime);
                Debug.Log("Audio: Snapshot Normal Restaurado");
            }
        }
    }
}
