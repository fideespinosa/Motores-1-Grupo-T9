using System.Collections.Generic;
using UnityEngine;

public class StoryFlagManager : MonoBehaviour
{
    public static StoryFlagManager Instance { get; private set; }

    private readonly HashSet<string> flags = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetFlag(string flagId)
    {
        if (string.IsNullOrEmpty(flagId)) return;
        flags.Add(flagId);
    }

    public bool HasFlag(string flagId)
    {
        if (string.IsNullOrEmpty(flagId)) return false;
        return flags.Contains(flagId);
    }
}