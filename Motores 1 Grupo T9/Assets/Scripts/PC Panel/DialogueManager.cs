using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    public string dialogueText;
    public int priority;
    public bool needsCard;
    public bool needsHDD;
    public bool needsPhone;
    public bool needsPC;
}