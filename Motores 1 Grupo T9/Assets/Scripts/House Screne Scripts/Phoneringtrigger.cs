using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PhoneRingTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [Tooltip("Segundos de espera desde que el jugador toca el collider hasta que el teléfono empieza a sonar.")]
    [SerializeField] private float delayBeforeRinging = 3f;
    [SerializeField] private Phone phone;

    private bool triggered = false;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag(playerTag)) return;

        triggered = true;
        StartCoroutine(RingAfterDelay());
    }

    private IEnumerator RingAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeRinging);

        if (phone != null)
        {
            phone.StartRingingSequence();
        }
    }
}