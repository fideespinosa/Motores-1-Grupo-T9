using UnityEngine;

public class TransitionSFXController : MonoBehaviour
{
    [Header("Transición DRON/SHIP")]
    [SerializeField] private AudioSource transitionSource;

    [Tooltip("ToDron")]
    [SerializeField] private AudioClip toDroneClip;

    [Tooltip("ToShip")]
    [SerializeField] private AudioClip toShipClip;

    public void PlayTransition(bool isGoingToDrone)
    {
        if (transitionSource == null) return;

        AudioClip clipToPlay = isGoingToDrone ? toDroneClip : toShipClip;

        if (clipToPlay != null)
        {
            transitionSource.PlayOneShot(clipToPlay);
        }
    }
}