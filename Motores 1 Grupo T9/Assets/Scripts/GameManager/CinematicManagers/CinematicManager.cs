
using System.Collections;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private EnemyBehaviorLVL2 enemy;
    [SerializeField] private Transform enemyLookPoint;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private CameraSequenceController cameraSequence;
    [SerializeField] private FadeBehaviorScript panelScript;
    [SerializeField] private PlayerMovement PlayScript;
    private CharacterController playerController;

    [Header("Retroceso")]
    [SerializeField] private float backwardSpeed = 1.5f;
    [SerializeField] private float lookSmooth = 10f;
    [SerializeField] private float maxDuration = 15f;


    private bool cinematicPlaying;
    private bool lockLook;

    private bool previousMovementEnabled;
    private bool previousCameraSequenceEnabled;

    private Rigidbody rb;

    private void Awake()
    {
        if (playerRoot != null)
        {
            rb = playerRoot.GetComponent<Rigidbody>();
            playerController = playerRoot.GetComponent<CharacterController>();
        }
    }

    private void LateUpdate()
    {
        if (!lockLook || playerCamera == null || enemyLookPoint == null)
            return;

        // Único control de orientación de la cámara durante la cinemática.
        Vector3 direction = enemyLookPoint.position - playerCamera.transform.position;
        Debug.Log(
    $"Tiempo: {Time.time:F2} | " +
    $"EnemyLookPoint: {enemyLookPoint.position} | " +
    $"Cámara: {playerCamera.transform.position}"
);

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        playerCamera.transform.rotation = Quaternion.Slerp(
            playerCamera.transform.rotation,
            targetRotation,
            lookSmooth * Time.deltaTime
        );
    }

    public void StartMonsterCinematic()
    {
        if (cinematicPlaying)
            return;

        StartCoroutine(MonsterSequence());
    }

    private IEnumerator MonsterSequence()
    {
        Debug.Log($"Inicio cinemática: ScreamEnded = {enemy.ScreamEnded}");
        cinematicPlaying = true;

        // Guardar los estados originales para restaurarlos al terminar.
        if (PlayScript != null)
        {
            previousMovementEnabled = PlayScript.enabled;
            PlayScript.enabled = false;
        }

        if (cameraSequence != null)
        {
            previousCameraSequenceEnabled = cameraSequence.enabled;

            // Detener cualquier secuencia anterior de cámara.
            cameraSequence.StopLook();
            cameraSequence.enabled = false;
        }

        if (rb != null)
        {
            if (!rb.isKinematic)
                rb.linearVelocity = Vector3.zero;

            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        // Desde acá, este script controla la orientación de la cámara.
        lockLook = true;

        enemy.StartScreaming();

        float elapsed = 0f;

        while (!enemy.ScreamEnded && elapsed < maxDuration)
        {
            elapsed += Time.fixedDeltaTime;

            // Girar el jugador horizontalmente hacia el enemigo.
            if (enemyLookPoint != null)
            {
                Vector3 toEnemy = enemyLookPoint.position - playerRoot.position;
                toEnemy.y = 0f;

                if (toEnemy.sqrMagnitude > 0.001f)
                {
                    Quaternion targetYaw =
                        Quaternion.LookRotation(toEnemy.normalized);

                    Quaternion currentRotation =
                        rb != null ? rb.rotation : playerRoot.rotation;

                    Quaternion newRotation = Quaternion.Slerp(
                        currentRotation,
                        targetYaw,
                        lookSmooth * Time.fixedDeltaTime
                    );

                    if (rb != null)
                        rb.MoveRotation(newRotation);
                    else
                        playerRoot.rotation = newRotation;
                }
            }

            // Retroceder alejándose del enemigo.
            Vector3 away = playerRoot.position - enemy.transform.position;
            away.y = 0f;

            if (away.sqrMagnitude > 0.001f)
            {
                Vector3 step =
                    away.normalized * backwardSpeed * Time.fixedDeltaTime;

                if (playerController != null && playerController.enabled)
                {
                    playerController.Move(step);
                }
                else
                {
                    playerRoot.position += step;
                }
            }
            Debug.Log($"Esperando Scream: {enemy.ScreamEnded}, tiempo: {elapsed:F2}");
            yield return new WaitForFixedUpdate();
        }

        Debug.Log($"Cinemática termina. ScreamEnded: {enemy.ScreamEnded}");
        // Detener el movimiento y liberar la cámara.
        lockLook = false;

        if (rb != null && !rb.isKinematic)
            rb.linearVelocity = Vector3.zero;

        if (panelScript != null)
            panelScript.StartFade();

        // Sincronizar la orientación del jugador con la cámara actual.
        if (PlayScript != null && playerCamera != null)
            PlayScript.SetLookDirection(playerCamera.transform.forward);

        // Restaurar los controles que estaban activos antes.
        if (cameraSequence != null)
            cameraSequence.enabled = previousCameraSequenceEnabled;

        if (PlayScript != null)
            PlayScript.enabled = previousMovementEnabled;

        cinematicPlaying = false;

        Debug.Log("Cinemática terminada. Control devuelto.");
    }
}