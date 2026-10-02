using System.Collections;
using UnityEngine;

public class CameraPointSwitcher : MonoBehaviour
{
    private enum StartPoint
    {
        PointA,
        PointB
    }
    [Header("Image")]
    [SerializeField] private TVImageSequence imageScript;

    [Header("Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private Transform pointC;
    [SerializeField] private StartPoint startingPoint = StartPoint.PointA;

    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.4f;

    [Header("Escape Toggle")]
    [SerializeField] private bool enableEscapeToggle = false;

    [Header("References")]
    [SerializeField] private MonoBehaviour fpsOldInput;
    [SerializeField] private SubtitleSequencePlayer subScript;

    [Header("Initial Point B Sequence")]
    [SerializeField] private float waitAtPointB = 3f;
    [SerializeField] private float waitAtPointC = 3f;
    [SerializeField] private CanvasGroup continuePanel;
    [SerializeField] private GameObject hud;

    [Header("Panel Fade")]
    [SerializeField] private float panelFadeDuration = 1f;

    private bool isAtPointA;
    private bool isMoving = false;
    private bool sequenceRunning = false;

    private void Start()
    {
        isAtPointA = startingPoint == StartPoint.PointA;

        Transform target = isAtPointA ? pointA : pointB;

        if (target != null)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }

        if (fpsOldInput != null)
            fpsOldInput.enabled = isAtPointA;

        if (hud != null)
            hud.SetActive(isAtPointA);

        if (continuePanel != null)
        {
            continuePanel.alpha = 0f;
            continuePanel.gameObject.SetActive(false);
        }

        if (startingPoint == StartPoint.PointB)
            StartCoroutine(PointBSequence());
    }

    public void SwitchCamera()
    {
        if (isMoving || sequenceRunning) return;

        StartCoroutine(MoveTo(pointB, false));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
        {
            if (!isAtPointA && enableEscapeToggle && !sequenceRunning)
            {
                StartCoroutine(MoveTo(pointA, true));
            }
        }
    }

    private IEnumerator PointBSequence()
    {
        sequenceRunning = true;

        yield return new WaitForSeconds(waitAtPointB);

        bool spacePressed = false;

        if (continuePanel != null)
        {
            continuePanel.gameObject.SetActive(true);
            continuePanel.alpha = 0f;

            float elapsed = 0f;

            while (elapsed < panelFadeDuration)
            {
                elapsed += Time.deltaTime;
                continuePanel.alpha = Mathf.Lerp(0f, 1f, elapsed / panelFadeDuration);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    FindFirstObjectByType<TypewritterText>().ForceStopAndFinish();
                    spacePressed = true;
                    break;
                }

                yield return null;
            }

            if (!spacePressed)
            {
                continuePanel.alpha = 1f;
            }
        }

        if (!spacePressed)
        {
            
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            FindFirstObjectByType<TypewritterText>().ForceStopAndFinish();
        }

        if (continuePanel != null)
        {
            float elapsed = 0f;
            float startAlpha = continuePanel.alpha;

            while (elapsed < panelFadeDuration)
            {
                elapsed += Time.deltaTime;
                continuePanel.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / panelFadeDuration);
                yield return null;
            }

            continuePanel.alpha = 0f;
            continuePanel.gameObject.SetActive(false);
        }

        yield return StartCoroutine(MoveTo(pointC, false));
        
        // aca poner audio de la ia
        subScript.Play();
        imageScript.StartSequence();

        yield return new WaitForSeconds(waitAtPointC);

        yield return StartCoroutine(MoveTo(pointA, true));

        enableEscapeToggle = !enableEscapeToggle;
        moveDuration = 0.4f;

        sequenceRunning = false;
    }

    private IEnumerator MoveTo(Transform target, bool willBeAtPointA)
    {
        if (target == null) yield break;

        if (fpsOldInput != null)
            fpsOldInput.enabled = willBeAtPointA;

        if (hud != null)
            hud.SetActive(willBeAtPointA);

        isMoving = true;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);

            transform.position = Vector3.Lerp(startPos, target.position, t);
            transform.rotation = Quaternion.Slerp(startRot, target.rotation, t);

            yield return null;
        }

        transform.position = target.position;
        transform.rotation = target.rotation;

        isAtPointA = willBeAtPointA;
        isMoving = false;
    }
}