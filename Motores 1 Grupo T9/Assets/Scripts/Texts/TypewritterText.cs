using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float pauseDuration = 0.5f;
    [SerializeField] private float dotPause = 0.8f;

    private string fullText;

    private void Start()
    {
        fullText = text.text;
        text.text = "";

        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        string currentText = "";

        for (int i = 0; i < fullText.Length; i++)
        {
            if (fullText[i] == '<')
            {
                int tagEnd = fullText.IndexOf('>', i);

                if (tagEnd != -1)
                {
                    string tag = fullText.Substring(i, tagEnd - i + 1);
                    currentText += tag;
                    text.text = currentText;

                    i = tagEnd;
                    continue;
                }
            }

            char letter = fullText[i];

            currentText += letter;
            text.text = currentText;
            //sonido de teclado por cada letra :)

            if (letter == ',')
            {
                yield return new WaitForSeconds(pauseDuration);
            }
            else if (letter == '.' || letter == ';' || letter == ':')
            {
                yield return new WaitForSeconds(dotPause);
            }
            else
            {
                yield return new WaitForSeconds(typingSpeed);
            }
        }
    }
}