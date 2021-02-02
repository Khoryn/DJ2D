using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CarController : MonoBehaviour
{
    [SerializeField]
    private float acceleration;
    [HideInInspector]
    public float initialSpeed;
    public float Acceleration
    {
        get { return acceleration; }
        set { acceleration = value; }
    }

    [SerializeField]
    private float steering;

    private float driftForce;

    public float DriftForce
    {
        get { return driftForce; }
    }

    private Rigidbody2D rb;

    public Vector2 startPosition;

    void Start()
    {
        initialSpeed = acceleration;
        transform.position = startPosition;

        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!GameState.IsPaused)
        {
            CarMovement();
        }
    }

    private void CarMovement()
    {
        float horizontalInput = -Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 speed = transform.up * (verticalInput * acceleration);
        rb.AddForce(speed);

        float direction = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up));

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

        Vector2 rightAngleFromForward = Quaternion.AngleAxis(steeringRightAngle, Vector3.forward) * forward;

        driftForce = Vector2.Dot(rb.velocity, rb.GetRelativeVector(rightAngleFromForward.normalized));

        Vector2 relativeForce = (rightAngleFromForward.normalized * -1.0f) * (driftForce * 10.0f);

        rb.AddForce(rb.GetRelativeVector(relativeForce));
    }
}