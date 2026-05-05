using UnityEditor;
using UnityEngine;

public class MinigamesManager : MonoBehaviour
{
    [Header("Minigames")]
    [SerializeField] GameObject lettersGame;

    [Header("UI Game")]
    [SerializeField] GameObject lettersPanel;

    [Header("Instructions Scripts")]
    [SerializeField] GameObject instructionsPanel;

    protected PlayerSwitcher switcher;

    public void StartLettersGame()
    {
        Debug.Log("lo llamo desde el manager");

        if (!instructionsPanel.GetComponent<InstructionsScript>().ShowInstructions())
        {
            lettersPanel.SetActive(true);
            lettersGame.SetActive(true);
        }
    }

    public void EndLettersGame()
    {

        lettersPanel.SetActive(false);
        lettersGame.SetActive(false);
    }

    public void UnfreezeGame()
    {
        if (switcher == null) switcher = Object.FindFirstObjectByType<PlayerSwitcher>();

        if (switcher != null)
        {

            switcher.enabled = true;

            switcher.SetControl(true);
        }


        transform.root.gameObject.SetActive(false);

        Debug.Log("Mundo descongelado y control devuelto al Dron desde el manager.");
    }

    public void FreezeGame()
    {
        if (switcher != null)
        {
            switcher.SetControl(false);
            switcher.enabled = false;
        }

        Debug.Log("Mundo congelado y desde el manager.");
    }

}
