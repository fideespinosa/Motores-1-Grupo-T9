using UnityEngine;

public class TransitionSFXController : MonoBehaviour
{
    [Header("Transición DRON/SHIP")]
    [SerializeField] private AudioSource transitionSource;

    [Tooltip("ToDron")]
    [SerializeField] private AudioClip toDroneClip;
    [SerializeField] private AudioClip ignitionClip;

    [Tooltip("ToShip")]
    [SerializeField] private AudioClip toShipClip;

    public void PlayTransition(bool isGoingToDrone)
    {
        if (transitionSource == null) return;

       // AudioClip clipToPlay = isGoingToDrone ? toDroneClip : toShipClip;

        if (isGoingToDrone == true && ignitionClip != null)
        {
            transitionSource.PlayOneShot(ignitionClip);

            if (toDroneClip != null)
            {
                transitionSource.PlayOneShot(toDroneClip);
            }
        }

        if (isGoingToDrone == false && toShipClip != null)
        {
            transitionSource.PlayOneShot(toShipClip);
        }
    }
}