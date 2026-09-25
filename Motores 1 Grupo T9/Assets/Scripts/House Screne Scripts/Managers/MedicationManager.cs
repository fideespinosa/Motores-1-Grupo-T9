using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MedicationRequirement
{
    public string medicationId;
    public string displayName;
    public int requiredAmount;
}

public class MedicationManager : MonoBehaviour
{
    public static MedicationManager Instance { get; private set; }

    [Header("Recipe")]
    [SerializeField] private List<MedicationRequirement> requirements;

    [Header("Story Flag")]
    [SerializeField] private string completedFlag = "medication_collected";

    private Dictionary<string, int> requiredAmounts = new Dictionary<string, int>();
    private Dictionary<string, int> collectedAmounts = new Dictionary<string, int>();
    private Dictionary<string, string> displayNames = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (MedicationRequirement requirement in requirements)
        {
            requiredAmounts[requirement.medicationId] = requirement.requiredAmount;
            displayNames[requirement.medicationId] = requirement.displayName;
            collectedAmounts[requirement.medicationId] = 0;
        }
    }

    public bool TryCollect(string medicationId)
    {
        if (!requiredAmounts.ContainsKey(medicationId))
        {
            ShowMessage("No, esto no es lo que busco.");
            return false;
        }

        if (collectedAmounts[medicationId] >= requiredAmounts[medicationId])
        {
            ShowMessage("No necesito mas de esto.");
            return false;
        }

        collectedAmounts[medicationId]++;

        if (IsComplete())
        {
            if (StoryFlagManager.Instance != null)
            {
                StoryFlagManager.Instance.SetFlag(completedFlag);
            }
            ShowMessage("Ya tengo todos los remedios que necesito. Deberia dejarle tambien un vaso con agua.");
        }
        else
        {
            string name = displayNames.ContainsKey(medicationId) ? displayNames[medicationId] : medicationId;
            ShowMessage("Aca está la " + name + ".");
        }

        return true;
    }

    public bool IsComplete()
    {
        foreach (KeyValuePair<string, int> pair in requiredAmounts)
        {
            if (collectedAmounts[pair.Key] < pair.Value)
            {
                return false;
            }
        }
        return true;
    }

    private void ShowMessage(string message)
    {
        if (TextPanelManager.Instance != null)
        {
            TextPanelManager.Instance.ShowText(message);
        }
    }
}