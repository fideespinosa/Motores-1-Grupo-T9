using UnityEngine;

public class CinematicAudioTrigger : MonoBehaviour
{
    private AudioSource myAudio;

    
    public float delayTime = 3.5f;

    void Start()
    {
        myAudio = GetComponent<AudioSource>();

        if (myAudio != null)
        {
           
            myAudio.PlayDelayed(delayTime);
        }
    }
}