using System.Collections;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
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

        cameraController.enabled = false;

        Vector3 originalPos = playerCamera.transform.position;
        Quaternion originalRot = playerCamera.transform.rotation;

        enemy.StartScreaming();

        float t = 0f;

        while (t < zoomDuration)
        {
            t += Time.deltaTime;

            playerCamera.transform.position = Vector3.Lerp(originalPos, monsterViewPoint.position, t / zoomDuration);

            playerCamera.transform.rotation = Quaternion.Lerp(originalRot,monsterViewPoint.rotation,t / zoomDuration);

            yield return null;
        }

        yield return new WaitForSeconds(monsterViewTime);

        t = 0f;

        while (t < zoomDuration)
        {
            t += Time.deltaTime;

            playerCamera.transform.position =
                Vector3.Lerp(monsterViewPoint.position,originalPos,t / zoomDuration);

            playerCamera.transform.rotation = Quaternion.Lerp(monsterViewPoint.rotation,originalRot, t / zoomDuration);

            yield return null;
        }

        cameraController.enabled = true;

        enemy.StartRunning();

        cinematicPlaying = false;
    }
}