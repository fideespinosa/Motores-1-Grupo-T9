using UnityEngine;
using System.Collections;

public class DronFlickerLight : MonoBehaviour
{
    [SerializeField] private Light spotLight;

    [SerializeField] private float minIntensity = 0.2f;
    [SerializeField] private float maxIntensity = 2f;

    [SerializeField] private float minWait = 0.03f;
    [SerializeField] private float maxWait = 0.15f;

    private void Start()
    {
        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            spotLight.intensity = Random.Range(minIntensity, maxIntensity);

            yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        }
    }
}