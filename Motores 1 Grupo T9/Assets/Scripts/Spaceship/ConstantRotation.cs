using Unity.VisualScripting;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [Header("Rotación")]
    [SerializeField] private Vector3 rotationAxis = new Vector3(0, 1, 0);
    [SerializeField] private float rotationSpeed = 360f;

    [Header("Audio config")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clockTick;
    [SerializeField] private AudioClip fanMovement;
    [SerializeField] private ObjectType currentObjectType;

    private void Start()
    {
        if (audioSource != null)
        {
            switch (currentObjectType)
            {
                case ObjectType.Clock:
                    audioSource.clip = clockTick;
                    break;
                case ObjectType.Fan:
                    audioSource.clip = fanMovement;
                    break;
            }
            audioSource.Play();
        }
    }

    public enum ObjectType
    {
        Clock,
        Fan
    }

    void Update()
    {
        transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime, Space.Self);
    }


}
