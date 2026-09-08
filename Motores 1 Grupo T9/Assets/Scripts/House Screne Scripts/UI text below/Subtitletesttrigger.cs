using UnityEngine;

public class SubtitleTestTrigger : MonoBehaviour
{
    [SerializeField] private SubtitleSequencePlayer subtitlePlayer;

    private void OnEnable()
    {
        if (subtitlePlayer != null)
        {
            subtitlePlayer.Play();
        }

        Debug.Log("test");
    }
}