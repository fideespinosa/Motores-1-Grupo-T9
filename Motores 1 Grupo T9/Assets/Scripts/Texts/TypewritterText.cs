using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TypewritterText : MonoBehaviour
{
    [Header("Pause Between Texts")]
    [SerializeField] private float pauseAfterText = 1f;

    [Header("Date")]
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private float dateTypingSpeed = 0.12f;

    [Header("Alert Blink")]
    [SerializeField] private TextMeshProUGUI alertLabel;
    [SerializeField] private float blinkInterval = 0.15f;
    [SerializeField] private float blinkDuration = 1f;
    [SerializeField] private float delayAfterBlink = 1f;

    [Header("Alert Text")]
    [SerializeField] private TextMeshProUGUI alertText;

    [Header("Fast Typing Speed")]
    [Tooltip("Usada para el texto de alerta, el título del log, y los textos de sistema.")]
    [SerializeField] private float fastTypingSpeed = 0.02f;

    [Header("Log")]
    [SerializeField] private TextMeshProUGUI logTitleText;
    [SerializeField] private TextMeshProUGUI logBodyText;
    [SerializeField] private float logTypingSpeed = 0.05f;
    [SerializeField] private float logPauseDuration = 0.5f;
    [SerializeField] private float logDotPause = 0.8f;

    [Header("System Texts")]
    [SerializeField] private List<TextMeshProUGUI> systemTexts;

    [Header("Audio")]
    [SerializeField] private AudioSource typingAudioSource;
    [SerializeField] private AudioClip cyberTypingClip;
    [SerializeField] private AudioClip humanTypingClip;
    private bool muteTypingAudio = false;

    [Header("Tutorial IA")]
    public GameObject iaTutorialObject;

    [Header("Evento para disparar IA tuto")]
    public UnityEvent onSequenceFinishedKeycap;

    private string dateFullText;
    private string alertFullText;
    private string logTitleFullText;
    private string logBodyFullText;
    private List<string> systemFullTexts = new List<string>();

    private void Awake()
    {
        dateFullText = dateText.text;
        dateText.text = "";

        alertFullText = alertText.text;
        alertText.text = "";

        logTitleFullText = logTitleText.text;
        logTitleText.text = "";

        logBodyFullText = logBodyText.text;
        logBodyText.text = "";

        if (alertLabel != null)
        {
            alertLabel.enabled = false;
        }

        foreach (TextMeshProUGUI t in systemTexts)
        {
            systemFullTexts.Add(t.text);
            t.text = "";
        }
    }

    private void Start()
    {
        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        yield return TypeOut(dateText, dateFullText, dateTypingSpeed, false, 0f, 0f);

        yield return BlinkLabel(alertLabel, blinkInterval, blinkDuration);
        yield return new WaitForSeconds(delayAfterBlink);

        yield return TypeOut(alertText, alertFullText, fastTypingSpeed, false, 0f, 0f);
        yield return new WaitForSeconds(pauseAfterText);

        yield return TypeOut(logTitleText, logTitleFullText, fastTypingSpeed, false, 0f, 0f);
        yield return new WaitForSeconds(pauseAfterText);

        yield return TypeOut(logBodyText, logBodyFullText, logTypingSpeed, true, logPauseDuration, logDotPause);
        yield return new WaitForSeconds(pauseAfterText);

        for (int i = 0; i < systemTexts.Count; i++)
        {
            yield return TypeOut(systemTexts[i], systemFullTexts[i], fastTypingSpeed, false, 0f, 0f);
        }

       //if (!muteTypingAudio && onSequenceFinishedKeycap != null)
       // {
        //    onSequenceFinishedKeycap.Invoke();
      //  }
    }

    private IEnumerator BlinkLabel(TextMeshProUGUI target, float interval, float duration)
    {
        if (target == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            target.enabled = !target.enabled;
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        target.enabled = true;
    }

    private IEnumerator TypeOut(TextMeshProUGUI target, string fullText, float speed, bool usePunctuationPauses, float commaPause, float dotPause)
    {
        string currentText = "";

        if (typingAudioSource != null && !muteTypingAudio)
        {
            typingAudioSource.clip = (target == logBodyText) ? humanTypingClip : cyberTypingClip;
            typingAudioSource.loop = true;
            typingAudioSource.Play();
        }

        for (int i = 0; i < fullText.Length; i++)
        {
            if (fullText[i] == '<')
            {
                int tagEnd = fullText.IndexOf('>', i);

                if (tagEnd != -1)
                {
                    string tag = fullText.Substring(i, tagEnd - i + 1);
                    currentText += tag;
                    target.text = currentText;

                    i = tagEnd;
                    continue;
                }
            }

            char letter = fullText[i];

            currentText += letter;
            target.text = currentText;

            if (usePunctuationPauses && letter == ',')
            {
                if (typingAudioSource != null) typingAudioSource.Pause();
                yield return new WaitForSeconds(commaPause);
                if (typingAudioSource != null && !muteTypingAudio) typingAudioSource.Play();
            }
            else if (usePunctuationPauses && (letter == '.' || letter == ';' || letter == ':'))
            {
                if (typingAudioSource != null) typingAudioSource.Pause();
                yield return new WaitForSeconds(dotPause);
                if (typingAudioSource != null && !muteTypingAudio) typingAudioSource.Play();
            }
            else
            {
                yield return new WaitForSeconds(speed);
            }
        }

        if (typingAudioSource != null) typingAudioSource.Stop();
    }

    public void ForceStopAndFinish()
    {
        if (muteTypingAudio) return;

        muteTypingAudio = true; 

        if (typingAudioSource != null)
        {
            typingAudioSource.Stop(); 
        }

        
        if (onSequenceFinishedKeycap != null)
        {
            onSequenceFinishedKeycap.Invoke();
        }
    }

}