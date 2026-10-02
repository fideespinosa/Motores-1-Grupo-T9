using System.Collections;
using UnityEngine;

public class CountdownBar : MonoBehaviour
{
    private enum Axis
    {
        X,
        Y,
        Z
    }

    [SerializeField] private Transform barPivot;
    [SerializeField] private Axis shrinkAxis = Axis.Y;

    private Vector3 fullScale;
    private Coroutine countdownCoroutine;

    private void Awake()
    {
        fullScale = barPivot.localScale;
    }

    public void StartCountdown(float duration)
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }

        countdownCoroutine = StartCoroutine(CountdownRoutine(duration));
    }

    public void ResetBar()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        barPivot.localScale = fullScale;
    }

    private IEnumerator CountdownRoutine(float duration)
    {
        barPivot.localScale = fullScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float remaining = 1f - Mathf.Clamp01(elapsed / duration);
            ApplyScale(remaining);
            yield return null;
        }

        ApplyScale(0f);
        countdownCoroutine = null;
    }

    private void ApplyScale(float t)
    {
        Vector3 scale = fullScale;

        if (shrinkAxis == Axis.X)
        {
            scale.x = fullScale.x * t;
        }
        else if (shrinkAxis == Axis.Y)
        {
            scale.y = fullScale.y * t;
        }
        else
        {
            scale.z = fullScale.z * t;
        }

        barPivot.localScale = scale;
    }
}