using System.Collections;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    [SerializeField] Transform cameraPlace;
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform monsterViewPoint;
    [SerializeField] EnemyBehaviorLVL2 enemy;

    [SerializeField] float zoomDuration = 0.01f;
    [SerializeField] float monsterViewTime = 1.5f;

    private CameraController cameraController;
    private bool cinematicPlaying = false;

    void Start()
    {
        cameraController = playerCamera.GetComponent<CameraController>();
    }

    public void StartMonsterCinematic()
    {
        if (cinematicPlaying)
            return;

        StartCoroutine(MonsterSequence());
    }

    IEnumerator MonsterSequence()
    {
        cinematicPlaying = true;

        if (cameraController != null)
            cameraController.enabled = false;

        enemy.StartScreaming();

        float t = 0f;

        Vector3 startPos = playerCamera.transform.position;
        Quaternion startRot = playerCamera.transform.rotation;

        while (t < zoomDuration)
        {
            t += Time.deltaTime;

            playerCamera.transform.position = Vector3.Lerp(
                startPos,
                monsterViewPoint.position,
                t / zoomDuration);

            playerCamera.transform.rotation = Quaternion.Lerp(
                startRot,
                monsterViewPoint.rotation,
                t / zoomDuration);

            yield return null;
        }

        yield return new WaitForSeconds(monsterViewTime);

        t = 0f;

        while (t < zoomDuration)
        {
            t += Time.deltaTime;

            playerCamera.transform.position = Vector3.Lerp(
                monsterViewPoint.position,
                cameraPlace.position,
                t / zoomDuration);

            playerCamera.transform.rotation = Quaternion.Lerp(
                monsterViewPoint.rotation,
                cameraPlace.rotation,
                t / zoomDuration);

            yield return null;
        }

        playerCamera.transform.position = cameraPlace.position;
        playerCamera.transform.rotation = cameraPlace.rotation;

        if (cameraController != null)
            cameraController.enabled = true;

        enemy.StartRunning();

        cinematicPlaying = false;
    }
}