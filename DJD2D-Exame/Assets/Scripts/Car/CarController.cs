using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CarController : MonoBehaviour
{
    [HideInInspector]
    public float initialSpeed;

    [Header("Velocity")]
    [SerializeField]
    private float acceleration;

    public float Acceleration
    {
        get { return acceleration; }
        set { acceleration = value; }
    }

    [Header("Steering")]
    [SerializeField]
    private float steering;

    private Rigidbody2D rb;

    private float driftForce;

    public float DriftForce
    {
        get { return driftForce; }
    }

    void Start()
    {
        // Set the player's initial speed
        initialSpeed = acceleration;

        // Get the player's rigidbody component
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // If the game isn't paused, allow the car to move
        if (!GameState.IsPaused)
        {
            CarMovement();
        }
    }

    private void CarMovement()
    {
        // Get the horizontal and vertical input
        float horizontalInput = -Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Set the player's speed based on it's input and acceleration
        Vector2 speed = transform.up * (verticalInput * acceleration);
        rb.AddForce(speed);

        // Set the direction
        float direction = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up));

        // Rotate the riggidbody based on input
        if (direction >= 0.0f)
        {
            rb.rotation += horizontalInput * steering * (rb.velocity.magnitude / 5.0f);
        }
        else
        {
            rb.rotation -= horizontalInput * steering * (rb.velocity.magnitude / 5.0f);
        }

        Vector2 forward = new Vector2(0.0f, 0.5f);

        float steeringRightAngle;

        if (rb.angularVelocity > 0)
        {
            steeringRightAngle = -90;
        }
        else
        {
            steeringRightAngle = 90;
        }

        // Set the player's drift based on it's current steering
        Vector2 rightAngleFromForward = Quaternion.AngleAxis(steeringRightAngle, Vector3.forward) * forward;

        driftForce = Vector2.Dot(rb.velocity, rb.GetRelativeVector(rightAngleFromForward.normalized));

        Vector2 relativeForce = (rightAngleFromForward.normalized * -1.0f) * (driftForce * 10.0f);

        rb.AddForce(rb.GetRelativeVector(relativeForce));
    }
}