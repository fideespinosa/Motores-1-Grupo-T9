using System.Collections;
using UnityEngine;
using TMPro;

public class TypingEffect : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string texto;
    public float demoraPorLetra = 0.25f;

    void Start()
    {
        textComponent.text = "";
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        foreach (char caracter in texto)
        {
            textComponent.text += caracter;
            yield return new WaitForSeconds(demoraPorLetra);
        }
    }
}