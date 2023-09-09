using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickPlayerExample : MonoBehaviour
{
    //public float speed;
    //public VariableJoystick variableJoystick;
    //public Rigidbody rb;

    //public void FixedUpdate()
    //{
    //    Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
    //    rb.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    //}

    public Transform cameraTransform;
    public Joystick joystick;
    public float moveSpeed;
    private void Update()
    {
        //Get the input direction
        float inputX = joystick.Horizontal;
        float inputY = joystick.Vertical;
        Vector3 inputDirection = new Vector3(inputX, inputY, 0);

        cameraTransform.Translate(inputDirection * moveSpeed * Time.deltaTime);
    }
}