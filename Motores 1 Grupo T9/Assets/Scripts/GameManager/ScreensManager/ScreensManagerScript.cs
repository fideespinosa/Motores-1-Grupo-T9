using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class ScreensManagerScript : MonoBehaviour
{
    int currentLevel = 1;
    public static event Action OnScreenActive;
    public static event Action OnScreenInactive;
    [Header("Screens")]
    [SerializeField] private GameObject Screen1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void NextLevel()
    {
        // no cambiar de escena en implementacion final, sino que mueve al dron al nuevo nivel.
        // falta validacion de recursos

        switch (currentLevel)
        {
            case 1:
                Debug.Log("Nivel 1");

                SceneManager.LoadScene("Victory");
                currentLevel++;
                break;

            case 2:
                Debug.Log("Nivel 2");
                currentLevel++;
                break;

            case 3:
                Debug.Log("Nivel 3");
                currentLevel++;
                break;

            case 4:
                Debug.Log("Nivel 4");
                currentLevel++;
                break;

            case 5:
                Debug.Log("Nivel 5");
                currentLevel++;
                break;

            default:
                Debug.Log("Nivel no válido");
                break;
        }

    }

    public void OpenPanelScreen1()
    {
        Screen1.SetActive(true);
        Cursor.visible = true;
        OnScreenActive?.Invoke();
    }
    public void ClosePanelScreen1()
    {
        Screen1.SetActive(false);
        Cursor.visible = false;
        OnScreenInactive?.Invoke();
    }
}
