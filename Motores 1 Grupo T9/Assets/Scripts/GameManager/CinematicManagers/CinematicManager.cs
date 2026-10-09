using System.Collections;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] EnemyBehaviorLVL2 enemy;
    [SerializeField] Transform enemyLookPoint;   // punto en la cabeza/pecho del enemigo
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform playerRoot;       // el objeto Player
    [SerializeField] Transform cameraPivot;      // el CameraPivot
    [SerializeField] CameraSequenceController cameraSequence; // el hijo del Player
    [SerializeField] FadeBehaviorScript panelScript;
    [SerializeField] private PlayerMovement PlayScript;

    [Header("Retroceso")]
    [SerializeField] float backwardSpeed = 1.5f;
    [SerializeField] float lookSmooth = 10f;
    [SerializeField] float maxDuration = 15f;    // seguro por si algo falla

    private CameraController cameraController;
    private bool cinematicPlaying = false;
    private bool lockLook = false;
    float lastPitch;

    void Start()
    {
        // busca el CameraController donde esté (cámara, pivot o player)
        cameraController = playerCamera.GetComponentInParent<CameraController>();
        if (cameraController == null)
            cameraController = playerRoot.GetComponentInChildren<CameraController>(true);
    }

    // Inclinación vertical: el pivot apunta al enemigo después de todos los demás scripts
    void LateUpdate()
    {
       
        if (!lockLook || cameraPivot == null) return;

        Vector3 toEnemy = enemyLookPoint.position - cameraPivot.position;
        Vector3 local = cameraPivot.parent.InverseTransformDirection(toEnemy);
        float flat = new Vector2(local.x, local.z).magnitude;
        float pitch = -Mathf.Atan2(local.y, flat) * Mathf.Rad2Deg;
        pitch = Mathf.Clamp(pitch, -25f, 25f);

        if (Mathf.Abs(pitch - lastPitch) > 8f)
            Debug.Log($"salto pitch {lastPitch:F1} -> {pitch:F1} | distancia {toEnemy.magnitude:F2}");
        lastPitch = pitch;

        cameraPivot.localRotation = Quaternion.Slerp(
            cameraPivot.localRotation,
            Quaternion.Euler(pitch, 0f, 0f),
            lookSmooth * Time.deltaTime);
    }

    public void StartMonsterCinematic()
    {
        Debug.Log("StartMonsterCinematic llamado");
        if (cinematicPlaying)
            return;

        StartCoroutine(MonsterSequence());
    }

    IEnumerator MonsterSequence()
    {
        Debug.Log("MonsterSequence empezó");
        cinematicPlaying = true;

        PlayScript.enabled = false;
        if (cameraController != null) cameraController.enabled = false;
        if (cameraSequence != null) cameraSequence.enabled = false;

        Rigidbody rb = playerRoot.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (!rb.isKinematic)
                rb.linearVelocity = Vector3.zero; // si tu Unity es anterior a la 6, usá rb.velocity
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        lockLook = true;
        enemy.StartScreaming();

        float elapsed = 0f;

        // mientras el enemigo hace el intro: el jugador lo mira y retrocede
        while (!enemy.IntroDone && elapsed < maxDuration)
        {
            elapsed += Time.fixedDeltaTime;

            // giro horizontal del Player hacia el enemigo
            Vector3 toEnemy = enemyLookPoint.position - playerRoot.position;
            toEnemy.y = 0f;
            if (toEnemy.sqrMagnitude > 0.001f)
            {
                Quaternion targetYaw = Quaternion.LookRotation(toEnemy.normalized);
                Quaternion newRot = Quaternion.Slerp(
                    rb != null ? rb.rotation : playerRoot.rotation,
                    targetYaw,
                    lookSmooth * Time.fixedDeltaTime);

                if (rb != null) rb.MoveRotation(newRot);
                else playerRoot.rotation = newRot;
            }

            // retroceder alejándose del enemigo
            Vector3 away = playerRoot.position - enemy.transform.position;
            away.y = 0f;
            Vector3 step = away.normalized * backwardSpeed * Time.fixedDeltaTime;

            if (rb != null) rb.MovePosition(rb.position + step);
            else playerRoot.position += step;

            yield return new WaitForFixedUpdate();
        }

        lockLook = false;

        if (rb != null && !rb.isKinematic)
            rb.linearVelocity = Vector3.zero; // si tu Unity es anterior a la 6, usá rb.velocity

        panelScript.StartFade();
        Debug.Log("fade del shift");

        // devolver el control sin que la cámara pegue un salto
        PlayScript.SetLookDirection(playerCamera.transform.forward);

        if (cameraController != null) cameraController.enabled = true;
        if (cameraSequence != null) cameraSequence.enabled = true;
        PlayScript.enabled = true;

        cinematicPlaying = false;
    }
}