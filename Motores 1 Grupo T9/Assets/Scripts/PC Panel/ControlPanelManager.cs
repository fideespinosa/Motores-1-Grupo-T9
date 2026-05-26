// ControlPanelManager.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ControlPanelManager : MonoBehaviour
{
    public static ControlPanelManager Instance { get; private set; }

    [Header("UI")]
    public GameObject panel;
    public TextMeshProUGUI dialogueText;
    public GameObject showObjectOnPC;

    [Header("Recursos narrativos recolectados")]
    public bool hasHDD;
    public bool hasSmartphone;
    public bool hasPC;
    public bool hasCard;

    [Header("Diálogos")]
    public List<DialogueEntry> defaultDialogues;

    private int currentDialogueIndex = 0;
    private List<DialogueEntry> dialogues = new List<DialogueEntry>();

    [Header("Escritura")]
    public float typingSpeed = 0.15f;
    private Coroutine typingCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        defaultDialogues = new List<DialogueEntry>
        {
            new DialogueEntry {
                dialogueText = "Esto parece una Tarjeta...",
                priority = 0,
                needsCard = true
            },
            new DialogueEntry {
                dialogueText = "Qué extraño encontrar esto aquí... El disco duro tiene archivos encriptados...",
                priority = 3,
                needsHDD = true
            },
            new DialogueEntry {
                dialogueText = "¡Un teléfono celular! Es realmente antiguo...",
                priority = 2,
                needsPhone = true
            },
            new DialogueEntry {
                dialogueText = "¿Cómo puede existir esto aqui?",
                priority = 1,
                needsPC = true,
            },
            new DialogueEntry {
                dialogueText = "Con los tres objetos la verdad es clara...",
                priority = 4,
                needsHDD = true,
                needsPhone = true,
                needsPC = true
            },
        };
    }

    public void ShowCurrentDialogue()
    {

        List<DialogueEntry> desbloqueados = new List<DialogueEntry>();

        foreach (DialogueEntry dialogo in defaultDialogues)
        {
            if (IsUnlocked(dialogo))
            {
                desbloqueados.Add(dialogo);
            }
        }

        for (int i = 0; i < desbloqueados.Count - 1; i++)
        {
            for (int j = 0; j < desbloqueados.Count - 1 - i; j++)
            {
                if (desbloqueados[j].priority > desbloqueados[j + 1].priority)
                {
                    DialogueEntry temp = desbloqueados[j];
                    desbloqueados[j] = desbloqueados[j + 1];
                    desbloqueados[j + 1] = temp;
                }
            }
        }

        dialogues = desbloqueados;
        currentDialogueIndex = 0;

        if (dialogues.Count > 0)
        {
            panel.SetActive(true);
            MostrarTexto(dialogues[0].dialogueText);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            panel.SetActive(false);
        }
    }

    public void NextDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex < dialogues.Count)
        {
            MostrarTexto(dialogues[currentDialogueIndex].dialogueText);
        }
        else
        {
            panel.SetActive(false);
            showObjectOnPC.gameObject.SetActive(false);
            currentDialogueIndex = 0;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    bool IsUnlocked(DialogueEntry entradaDialogo)
    {
        if (entradaDialogo.needsCard && !hasCard) return false;
        if (entradaDialogo.needsHDD && !hasHDD) return false;
        if (entradaDialogo.needsPhone && !hasSmartphone) return false;
        if (entradaDialogo.needsPC && !hasPC) return false;
        return true;
    }

    public void RecollectCard()
    {
        hasCard = true;
    }
    public void RecollectHDD() { 
        hasHDD = true; 
    }

    public void RecollectSmartphone() {
        hasSmartphone = true; 
    }

    public void ReollectPC() { 
        hasPC = true; 
    }

    void MostrarTexto(string texto)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(EscribirCaracterAPorCaracter(texto));
    }
    IEnumerator EscribirCaracterAPorCaracter(string texto)
    {
        dialogueText.text = string.Empty;

        foreach (char caracter in texto)
        {
            dialogueText.text += caracter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}