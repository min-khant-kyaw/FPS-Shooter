using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    private Vector3 lastPosition = new Vector3(0f,0f,0f);
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        // Resetting the default velocity
        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        // Getting the inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Creating the moving vector
        Vector3 move = transform.right * x + transform.forward * z; // (right - red axis, forward - yellow axis)
        
        // Actually moving the player
        controller.Move(move * speed * Time.deltaTime);

        // Check if player can jump
        if (Input.GetButtonDown("Jump") && isGrounded) {
            // Actually jumping
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Falling down
        velocity.y += gravity * Time.deltaTime;

        // Executing the jump
        controller.Move(velocity * Time.deltaTime);
        
        lastPosition = gameObject.transform.position;
    }
}
