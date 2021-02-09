using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarMovement : MonoBehaviour
{
    [Header("AI Path")]
    [SerializeField]
    private Transform path;

    [Header("Steering")]
    [SerializeField]
    private float maxSteerAngle = 45f;

    [Header("Velocity")]
    [SerializeField]
    private float maxSpeed;

    public float MaxSpeed
    {
        get { return maxSpeed; }
        set { maxSpeed = value; }
    }

    [Header("Initial Velocity")]
    [SerializeField]
    private float initialSpeed;

    public float InitialSpeed
    {
        get { return initialSpeed; }
        set { initialSpeed = value; }
    }

    [Header("Missiles")]
    [SerializeField]
    private int missileCounter;

    [SerializeField]
    private GameObject missilePrefab;

    private bool canShoot = true;

    // Path nodes
    private List<Transform> nodes;
    private int currentNode;

    private Rigidbody2D rb;

    // Start Position
    public Vector2 startPosition;

    // References
    CarController player;

    private void Start()
    {
        player = FindObjectOfType<CarController>();

        initialSpeed = maxSpeed;

        transform.position = startPosition;

        rb = GetComponent<Rigidbody2D>();

        Transform[] pathTransform = path.GetComponentsInChildren<Transform>(); // Get all the nodes inside the path object

        nodes = new List<Transform>();

        // Get the path nodes
        for (int i = 0; i < pathTransform.Length; i++)
        {
            if (pathTransform[i] != path.transform)
            {
                nodes.Add(pathTransform[i]);
            }
        }
    }

    private void Update()
    {
        FireProjectile();
        MoveMissile();
    }

    private void FixedUpdate()
    {
        if (!GameState.IsPaused) // If the race has begun, then move, steer and set waypoints for the AI
        {
            ApplySteer();
            Drive();
            CheckWaypointsDistance();
        }
    }

    private void ApplySteer()
    {
        // Rotate AI based on the current's node direction
        Vector3 vectorToTarget = nodes[currentNode].transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle + maxSteerAngle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, 0.2f);
    }

    private void Drive()
    {
        // Move AI to the current node
        Vector3 direction = (nodes[currentNode].transform.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction.normalized * maxSpeed * Time.fixedDeltaTime);
        rb.angularVelocity = 0f;
    }

    private void CheckWaypointsDistance()
    {
        // Check distance from AI to the current node, increment if distance is smaller than 15
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 15f)
        {
            if (currentNode == nodes.Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode++;
            }
        }
    }

    private bool IsInFront(GameObject p, GameObject a)
    {
        // Check if AI is in front or behind the player -> Must change later
        return Vector3.Dot(Vector3.up, p.transform.InverseTransformPoint(a.transform.position)) < 0;
    }

    public void FireProjectile()
    {
        if (!GameState.IsPaused)
        {
            if (IsInFront(player.gameObject, gameObject)) // if the player is in first place, the AI is allowed to shoot
            {
                if (missileCounter > 0 && canShoot) // Fire missile if counter is greater than 0 and if the AI can shoot
                {
                    StartCoroutine(Missile(5)); // Fire the missile
                    missileCounter--; // Reduce the missile counter
                    Debug.Log("Fire Missile");
                }
                else
                {
                    Debug.Log("Out of missiles");
                }
            }
        }
    }

    private IEnumerator Missile(float time)
    {
        canShoot = false;
        GameObject missile = Instantiate(missilePrefab, transform.position, Quaternion.identity); // Instantiate missile at the AI position
        yield return new WaitForSeconds(time); // Wait  before the AI is allowed to shoot again
        canShoot = true;
    }

    private void MoveMissile()
    {
        GameObject[] missiles = GameObject.FindGameObjectsWithTag("MissileAI");
        foreach (GameObject missile in missiles)
        {
            missile.transform.LookAt(player.transform.position); // Rotate missile to it looks at the player's transform
            missile.transform.Rotate(new Vector3(0, -90, 0), Space.Self);

            if (Vector3.Distance(missile.transform.position, player.transform.position) > 1) // Move missile while it's distance to player is greater than 1
            {
                missile.transform.Translate(new Vector3(45 * Time.deltaTime, 0, 0)); // Move the missile
            }
            else
            {
                StartCoroutine(ReduceSpeed(3)); // Reduce the player's current speed temporarily
                Destroy(missile); // Destroy the missile
            }
        }
    }

    IEnumerator ReduceSpeed(float time)
    {
        player.Acceleration -= 25; // Reduce the player's acceleration by 25
        yield return new WaitForSeconds(time);
        player.Acceleration = player.initialSpeed; // Reset the player's acceleration
    }
}
