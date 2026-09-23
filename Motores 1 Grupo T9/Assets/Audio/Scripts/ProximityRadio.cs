using UnityEngine;
using UnityEngine.Audio;

public class ProximityRadio : MonoBehaviour
{
    [Header("Snapshots del Mixer")]
    [SerializeField] private AudioMixerSnapshot snapshotNormal;
    [SerializeField] private AudioMixerSnapshot snapshotRadioON;

    [Tooltip("Tiempo en segundos que tarda en hacer el fade de volúmenes")]
    [SerializeField] private float tiempoDeTransicion = 1.5f;

    [Header("Configuración")]
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            snapshotRadioON.TransitionTo(tiempoDeTransicion);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
           
            snapshotNormal.TransitionTo(tiempoDeTransicion);
        }
    }
}