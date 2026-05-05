using System.Threading;
using UnityEngine;

public class LettersInstructionScript : InstructionsScript
{

    [SerializeField] MinigamesManager minigameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        minigameManager = minigameManager.GetComponent<MinigamesManager>();
    }
        
    public override void StartMinigame()
    {
        Debug.Log("llegamos?");
        minigameManager.StartLettersGame();
    }
    
}
