using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CaveToHouseScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject Panel;
    [SerializeField] DroneController droneController;
    [SerializeField] PlayerSwitcher playerSwitcher;
    [SerializeField] GameObject alarmLight;
    [SerializeField] private string triggerText;
    [SerializeField] private GameObject shipDoor;

    [SerializeField] private AudioSource[] ambientAndMusicSources;



    [SerializeField] ShipAlarmSFXController alarmSFXController;


    [Header("Transition")]
    [SerializeField] string SceneName;
    [SerializeField] float FadeDuration = 4f;

    private Image panelImage;
    private bool transitioning = false;

    void Start()
    {
        panelImage = Panel.GetComponent<Image>();

        Color color = panelImage.color;
        color.a = 0f;
        panelImage.color = color;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !transitioning)
        {
            StartTransition();
            shipDoor.GetComponent<BoxCollider>().enabled = true;
            shipDoor.GetComponent<Outline>().enabled = true;
        }

        if (SFXTransitionManager.Instance != null && ambientAndMusicSources.Length > 0)
        {
            SFXTransitionManager.Instance.FadeOutAudios(ambientAndMusicSources, FadeDuration);
        }
    }

    public void StartTransition()
    {
        if (transitioning)
            return;

        transitioning = true;

        droneController.enabled = false;
        playerSwitcher.enabled = false;

        Panel.SetActive(true);

        StartCoroutine(FadeToWhite());
    }

   
    IEnumerator FadeToWhite()
    {
        float elapsed = 0f;

        Color color = panelImage.color;
        color.a = 0f;

        while (elapsed < FadeDuration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(0f, 1f, elapsed / FadeDuration);

            color.a = alpha;
            panelImage.color = color;

            yield return null;
        }

        color.a = 1f;
        panelImage.color = color;

        alarmLight.SetActive(true);
        alarmSFXController.SetAlarmState(true);
        playerSwitcher.SetControl(false);

        if (!string.IsNullOrEmpty(triggerText) && TextPanelManager.Instance != null)
        {
            TextPanelManager.Instance.ShowText(triggerText);
        }
        // SceneManager.LoadScene(SceneName);
    }
}