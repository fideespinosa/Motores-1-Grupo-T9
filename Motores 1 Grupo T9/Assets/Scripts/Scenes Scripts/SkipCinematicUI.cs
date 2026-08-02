using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SkipCinematicUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup panel;

    [Header("Scene")]
    [SerializeField] private string sceneToLoad;

    [Header("Settings")]
    [SerializeField] private bool showOnlyOnInput = false;

    [SerializeField] private float firstAppearanceDelay = 8f;
    [SerializeField] private float fadeDuration = 0.7f;
    [SerializeField] private float visibleTime = 5f;
    [SerializeField] private float inputCooldown = 0.2f;

    private Coroutine routine;

    private bool initialSequenceFinished = false;
    private bool isVisible = false;

    private float lastInputTime;

    private void Start()
    {
        panel.alpha = 0f;

        if (showOnlyOnInput)
        {

            initialSequenceFinished = true;
        }
        else
        {
            routine = StartCoroutine(InitialSequence());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(sceneToLoad);
            return;
        }

        if (!initialSequenceFinished)
            return;

        bool mouseMoved =
            Mathf.Abs(Input.GetAxisRaw("Mouse X")) > 0.01f ||
            Mathf.Abs(Input.GetAxisRaw("Mouse Y")) > 0.01f;

        bool keyPressed = Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Space);

        if ((mouseMoved || keyPressed) &&
            Time.time - lastInputTime > inputCooldown)
        {
            lastInputTime = Time.time;

            if (routine != null)
                StopCoroutine(routine);

            routine = StartCoroutine(ShowPanel());
        }
    }

    private IEnumerator InitialSequence()
    {
        yield return new WaitForSeconds(firstAppearanceDelay);

        yield return ShowPanel();

        initialSequenceFinished = true;
    }

    private IEnumerator ShowPanel()
    {
        if (!isVisible)
        {
            yield return Fade(panel.alpha, 1f);
            isVisible = true;
        }

        yield return new WaitForSeconds(visibleTime);

        yield return Fade(panel.alpha, 0f);
        isVisible = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            panel.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }

        panel.alpha = to;
    }
}