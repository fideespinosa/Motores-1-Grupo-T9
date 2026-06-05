using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GlowOnClick : MonoBehaviour
{
    public GameObject glowEffect;
    public float pulseSpeed = 2f;
    public float glowDuration = 5f;

    void Start()
    {
        Invoke("DisableGlow", glowDuration);
    }

    void Update()
    {
        if (glowEffect.activeSelf)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            glowEffect.GetComponent<Image>().color = new Color(0, 1, 1, Mathf.Lerp(0.2f, 0.7f, t));
        }
    }

    void DisableGlow()
    {
        glowEffect.SetActive(false);
    }
}