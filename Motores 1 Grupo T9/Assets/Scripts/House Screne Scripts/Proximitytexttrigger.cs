using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProximityTextTrigger : MonoBehaviour
{
    [Header("Player Detection")]
    [SerializeField] private string playerTag = "Player";

    [Header("Text To Show")]
    [TextArea]
    [SerializeField] private string triggerText;

    [Header("Story Flag")]
    [Tooltip("Flag que se setea al dispararse. Dejar vacío si no aplica.")]
    [SerializeField] private string flagToSet;

    [Header("Settings")]
    [Tooltip("Si está activo, este trigger solo se dispara una vez en toda la partida.")]
    [SerializeField] private bool oneShot = true;

    private bool alreadyTriggered = false;


    private void OnTriggerEnter(Collider other)
    {

        // Debug.Log("colisiona");
        if (!other.CompareTag(playerTag)) return;
        if (oneShot && alreadyTriggered) return;

        alreadyTriggered = true;

        if (!string.IsNullOrEmpty(triggerText) && TextPanelManager.Instance != null)
        {
            TextPanelManager.Instance.ShowText(triggerText);
        }

        if (!string.IsNullOrEmpty(flagToSet) && StoryFlagManager.Instance != null)
        {
            StoryFlagManager.Instance.SetFlag(flagToSet);
        }
    }
}