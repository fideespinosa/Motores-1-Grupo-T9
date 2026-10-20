using System.Collections;
using UnityEngine;

public class LightSwitcher : MonoBehaviour, IInteractable
{
    [Header("Lights")]
    [SerializeField] private Light[] lights;
    [SerializeField] private bool startsOn = true;

    [Header("Switch Rotation")]
    [SerializeField] private Transform switchTransform;
    [SerializeField] private Vector3 rotationOffset = new Vector3(0f, 0f, 30f);
    [SerializeField] private float rotateDuration = 0.1f;

    [Header("Audio REF")]
    [SerializeField] private AudioSource lightSource;
    [SerializeField] private AudioClip switchClipON;
    [SerializeField] private AudioClip switchClipOFF;

    [Header("Requirement")]
    [SerializeField] private string requiredFlag;

    [Header("Outline")]
    [SerializeField] private Outline outline;

    private bool isOn;
    private bool isLocked;
    private Quaternion initialRotation;
    private Quaternion offsetRotation;
    private Coroutine rotateCoroutine;

    public bool IsLocked => isLocked;

    private Transform SwitchTarget
    {
        get { return switchTransform != null ? switchTransform : transform; }
    }

    private bool IsUnlocked()
    {
        if (string.IsNullOrEmpty(requiredFlag)) return true;
        return StoryFlagManager.Instance != null && StoryFlagManager.Instance.HasFlag(requiredFlag);
    }

    private void Awake()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }

        isOn = startsOn;
        initialRotation = SwitchTarget.localRotation;
        offsetRotation = initialRotation * Quaternion.Euler(rotationOffset);

        ApplyLights();
    }

    public void Action()
    {
        if (!IsUnlocked()) return;
        if (isLocked) return;

        isOn = !isOn;
        ApplyLights();

        MoveSwitchTo(GetSwitchRotation());

        if (lightSource != null)
        {
            lightSource.clip = isOn ? switchClipON : switchClipOFF;
            lightSource.Play();
        }
    }

    public void TurnOffAndLock()
    {
        if (isLocked) return;

        isLocked = true;
        isOn = false;
        ApplyLights();

        MoveSwitchTo(GetSwitchRotation());

        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void Unlock()
    {
        isLocked = false;
    }

    private Quaternion GetSwitchRotation()
    {
        return (isOn == startsOn) ? initialRotation : offsetRotation;
    }

    private void MoveSwitchTo(Quaternion target)
    {
        if (!isActiveAndEnabled) return;

        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
        }

        rotateCoroutine = StartCoroutine(RotateSwitch(target));
    }

    private void ApplyLights()
    {
        if (lights == null) return;

        foreach (Light lightSource in lights)
        {
            if (lightSource != null)
            {
                lightSource.enabled = isOn;
            }
        }
    }

    private IEnumerator RotateSwitch(Quaternion target)
    {
        Transform switchObject = SwitchTarget;

        if (rotateDuration <= 0f)
        {
            switchObject.localRotation = target;
            rotateCoroutine = null;
            yield break;
        }

        Quaternion start = switchObject.localRotation;
        float elapsed = 0f;

        while (elapsed < rotateDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / rotateDuration);
            switchObject.localRotation = Quaternion.Slerp(start, target, t);
            yield return null;
        }

        switchObject.localRotation = target;
        rotateCoroutine = null;
    }

    public void OnHoverEnter()
    {
        if (!IsUnlocked()) return;
        if (isLocked) return;

        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void OnHoverExit()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}