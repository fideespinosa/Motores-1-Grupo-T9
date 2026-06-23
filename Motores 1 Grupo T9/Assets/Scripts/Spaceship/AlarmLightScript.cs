using System.Reflection;
using UnityEngine;

public class AlarmLightScript : MonoBehaviour
{
    [SerializeField] Light alarmLight;
    [SerializeField] Light normalLight;

    [SerializeField] float minIntensity = 0f;
    [SerializeField] float maxIntensity = 8f;
    [SerializeField] float blinkSpeed = 8f;

    private void Update()
    {
        float intensity = Mathf.Lerp(
            minIntensity,
            maxIntensity,
            Mathf.PingPong(Time.time * blinkSpeed, 1)
        );

        alarmLight.intensity = intensity;

    }

    public void ActivateAlarm()
    {
        enabled = true;
        alarmLight.enabled = true;
    }

    public void DeactivateAlarm()
    {
        enabled = false;
        alarmLight.enabled = false;
        normalLight.enabled = true;
    }

    private void OnEnable()
    {
        normalLight.enabled = false;
        GameStatusScript.Instance.StartMinigame();
    }
    private void OnDisable()
    {
        normalLight.enabled = true;
        GameStatusScript.Instance.EndMinigame();
    }

}