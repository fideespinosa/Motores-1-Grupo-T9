using System.Collections;
using UnityEngine;

public class CableSparks : MonoBehaviour
{
    private ParticleSystem sparkParticles;

    private float minTime = 2f;
    private float maxTime = 5f;

    void Start()
    {
        sparkParticles = GetComponent<ParticleSystem>();
        StartCoroutine(Sparks());
    }

    IEnumerator Sparks()
    {
        while (true)
        {
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);
            sparkParticles.Play();
        }
    }
}
