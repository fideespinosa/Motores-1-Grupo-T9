using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionPanel : MonoBehaviour
{
    [System.Serializable]
    public class MissionStep
    {
        public string flag;
        [TextArea]
        public string missionText;
        public float delayBeforeAppearing = 0f;
    }

    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private TextMeshProUGUI missionText;
    [SerializeField] private List<MissionStep> missions;
    [SerializeField] private float fadeDuration = 0.5f;

    private int currentMissionIndex = -1;
    private Coroutine transitionCoroutine;

    private void Awake()
    {
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
        }
    }

    private void Update()
    {
        if (StoryFlagManager.Instance == null) return;

        int highestSatisfied = -1;
        for (int i = 0; i < missions.Count; i++)
        {
            if (StoryFlagManager.Instance.HasFlag(missions[i].flag))
            {
                highestSatisfied = i;
            }
        }

        if (highestSatisfied >= 0 && highestSatisfied != currentMissionIndex)
        {
            currentMissionIndex = highestSatisfied;

            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }

            transitionCoroutine = StartCoroutine(TransitionToMission(missions[currentMissionIndex].missionText, missions[currentMissionIndex].delayBeforeAppearing));
        }
    }

    private IEnumerator TransitionToMission(string newText, float delay)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        if (panelCanvasGroup.alpha > 0f)
        {
            yield return Fade(panelCanvasGroup.alpha, 0f, fadeDuration);
        }

        missionText.text = newText;

        yield return Fade(panelCanvasGroup.alpha, 1f, fadeDuration);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        panelCanvasGroup.alpha = to;
    }
}