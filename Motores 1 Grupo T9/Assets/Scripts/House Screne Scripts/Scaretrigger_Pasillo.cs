using System.Collections.Generic;
using UnityEngine;

public class ScareTrigger_Pasillo : MonoBehaviour
{
    [Header("Condiciones")]
    public string requiredKeyFlag = "red_key_picked_up";
    public string flagToSet = "PuertaBano_Cerrada";

    [Header("Referencias del Escenario")]
    public GameObject monsterModel;

    [Tooltip("Arrastra aquí todas las puertas que se deben cerrar.")]
    public List<Openable> doorsToClose = new List<Openable>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (StoryFlagManager.Instance == null) return;

        if (!StoryFlagManager.Instance.HasFlag(requiredKeyFlag)) return;

        if (StoryFlagManager.Instance.HasFlag(flagToSet)) return;

        StoryFlagManager.Instance.SetFlag(flagToSet);

        if (monsterModel != null)
        {
            // monsterModel.SetActive(true);
        }

        foreach (Openable door in doorsToClose)
        {
            if (door != null)
            {
                door.Close();
            }
        }
    }
}