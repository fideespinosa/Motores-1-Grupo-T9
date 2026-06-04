using UnityEngine;

public abstract class InstructionsScript : MonoBehaviour
{
    [SerializeField] GameObject InstructionPanel;
    [SerializeField] int count = 0;

    public abstract void StartMinigame(); // cada hijo debe implementar esta clase, diciendo que minijuego deberia empezar

    public bool ShowInstructions() // muestra las instrucciones, si es la primera vez que se muestran, devuelve true, sino devuelve false :)
    {
        if (count == 0)
        {
            
            Debug.Log("aca estamos");
/*            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;*/
            InstructionPanel.SetActive(true);
            count++;
            return true;
        }
        else
        {
            return false;
        }


    }
    public void ContinueButton()
    {
        InstructionPanel.SetActive(false);
        StartMinigame();
    }

}
