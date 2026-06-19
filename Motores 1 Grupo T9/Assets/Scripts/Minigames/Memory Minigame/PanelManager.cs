using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{

    [SerializeField] private int totalButtons = 5;
    [SerializeField] InterceptorScript interceptorScript;
    private int[] correctOrder;

    private bool puzzleCompleted = false;
    private int currentStep = 0;

    private List<PanelButton> pressedButtons = new();

    private void Start()
    {
        RandomizeOrder();
    }

    public void RandomizeOrder()
    {
        puzzleCompleted = false;
        currentStep = 0;

        foreach (PanelButton pressedButton in pressedButtons)
        {
            pressedButton.ResetVisual();
        }

        pressedButtons.Clear();

        correctOrder = new int[totalButtons];

        for (int i = 0; i < totalButtons; i++)
        {
            correctOrder[i] = i + 1;
        }

        for (int i = correctOrder.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            (correctOrder[i], correctOrder[randomIndex]) =
                (correctOrder[randomIndex], correctOrder[i]);
        }

    }

    public void PressButton(PanelButton button)
    {
        if (puzzleCompleted)
            return;

        int buttonNumber = button.GetButtonNumber();

        if (buttonNumber == correctOrder[currentStep])
        {
            Debug.Log("bien");

            button.SetPressed();
            pressedButtons.Add(button);

            currentStep++;

            if (currentStep >= correctOrder.Length)
            {
                interceptorScript.Win();
                Debug.Log("ganaste");
                puzzleCompleted = true;
            }
        }
        else
        {
            Debug.Log("mal");

            foreach (PanelButton pressedButton in pressedButtons)
            {
                pressedButton.ResetVisual();
            }

            pressedButtons.Clear();
            currentStep = 0;
        }
    }
}