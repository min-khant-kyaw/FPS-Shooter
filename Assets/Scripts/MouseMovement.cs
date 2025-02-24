using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 500f;

    float xRotation = 0f;
    float yRotation = 0f;
    public float topClamp = -45f;
    public float bottomClamp = 45f;


    void Start()
    {
        // Lock cursor in the middle of screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked;        
    }


    void Update()
    {
        // Getting the mouse inputs
        // Up and Down
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        // Left and Right
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotation around the x-axis (Look up and down)
        // Mouse mouse up, body rotates down and vice versa
        xRotation -= mouseY;

        // Clamp the rotation (sets limit so that funky things wont happen)
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        // Rotation around the y-axis (Look left and right)
        yRotation += mouseX;

        // Apply rotations to our transform
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
