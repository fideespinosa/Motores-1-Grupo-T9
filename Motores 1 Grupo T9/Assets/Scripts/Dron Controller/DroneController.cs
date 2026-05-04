using UnityEngine;
using UnityEngine.InputSystem;

public class DroneController : MonoBehaviour
{
    
    public RoverWheel[] leftWheels;
    public RoverWheel[] rightWheels;

    [SerializeField] private Light flashlight;

    public float motorForce = 1500f;
    public float turnForce = 1000f;

    private Vector2 inputMove;

   
    void Update()
    {
        float forward = 0;
        if (Keyboard.current.wKey.isPressed) forward = 1;
        if (Keyboard.current.sKey.isPressed) forward = -1;

        float turn = 0;
        if (Keyboard.current.dKey.isPressed) turn = 1;
        if (Keyboard.current.aKey.isPressed) turn = -1;

        inputMove = new Vector2(turn, forward);

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            flashlight.enabled = !flashlight.enabled;
        }
    }

    void FixedUpdate()
    {
        float leftPower = inputMove.y + inputMove.x;
        float rightPower = inputMove.y - inputMove.x;

        foreach (var wheel in leftWheels)
        {
            wheel.ApplyDriveForce(leftPower * motorForce);
        }

        foreach (var wheel in rightWheels)
        {
            wheel.ApplyDriveForce(rightPower * motorForce);
        }
    }
}