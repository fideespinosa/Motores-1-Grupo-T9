using UnityEngine;

/// che, las clases abstractas no son para esto. Solo para ser heredadas, no para ponerlas en los GameObjects, siempre te va a tirar alertas en la compilación
public abstract class InstructionsScript : MonoBehaviour
{
    [SerializeField] GameObject InstructionPanel;
    [SerializeField] int count = 0;

    public abstract void StartMinigame(); // cada hijo debe implementar esta clase, diciendo que minijuego deberia empezar

    public bool ShowInstructions() // muestra las instrucciones, si es la primera vez que se muestran, devuelve true, sino devuelve false :)
    {
        if (count == 0)
        {
            //Time.timeScale = 0f;
            Debug.Log("aca estamos");
            InstructionPanel.SetActive(true);
            Cursor.visible = true;
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
        //Time.timeScale = 1f;
        InstructionPanel.SetActive(false);
        StartMinigame();
    }

}
