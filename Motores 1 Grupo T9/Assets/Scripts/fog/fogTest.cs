using UnityEngine;
using System.Collections;

public class fogTest : MonoBehaviour
{
    [Header("Planos")]
    public Renderer[] planes;

    [Header("Tiempo")]
    public float delayBeforeFade = 0f;
    public float fadeDuration = 5f;

    private Material[] materials;
    private Color[] emissionColors;

    void Start()
    {
        materials = new Material[planes.Length];
        emissionColors = new Color[planes.Length];

        for (int i = 0; i < planes.Length; i++)
        {
            // Instancia del material para este objeto.
            materials[i] = planes[i].material;

            // Guardamos la emisión original.
            emissionColors[i] = materials[i].GetColor("_EmissionColor");
        }

        FadeOut();
    }

    public void FadeOut()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        if (delayBeforeFade > 0f)
        {
            yield return new WaitForSeconds(delayBeforeFade);
        }

        if (fadeDuration <= 0f)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetColor("_EmissionColor", Color.black);
                planes[i].enabled = false;
            }

            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / fadeDuration);

            for (int i = 0; i < materials.Length; i++)
            {
                Color emission = Color.Lerp(
                    emissionColors[i],
                    Color.black,
                    progress
                );

                materials[i].SetColor("_EmissionColor", emission);
            }

            yield return null;
        }
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetColor("_EmissionColor", Color.black);
            Destroy(planes[i].gameObject);
        }
    }
}