using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarMovement : MonoBehaviour
{
    public Transform path;
    public float maxSteerAngle = 45f;
    public float maxSpeed;
    public float initialSpeed;

    private List<Transform> nodes;
    private int currentNode;

    private Rigidbody2D rb;

    public Vector2 startPosition;

    public int missileCounter;

    public GameObject missilePrefab;

    bool canShoot = true;

    CarController player;

    private void Start()
    {
        player = FindObjectOfType<CarController>();

        initialSpeed = maxSpeed;

        transform.position = startPosition;

        rb = GetComponent<Rigidbody2D>();

        Transform[] pathTransform = path.GetComponentsInChildren<Transform>();

        nodes = new List<Transform>();

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
        if (!GameState.IsPaused)
        {
            ApplySteer();
            Drive();
            CheckWaypointsDistance();
        }
    }

    private void ApplySteer()
    {
        //Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        //float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        //rb.rotation = -newSteer;

        Vector3 vectorToTarget = nodes[currentNode].transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle + maxSteerAngle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, 0.2f);
    }

    private void Drive()
    {
        Vector3 direction = (nodes[currentNode].transform.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction.normalized * maxSpeed * Time.fixedDeltaTime);
        rb.angularVelocity = 0f;
    }

    private void CheckWaypointsDistance()
    {
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
        return Vector3.Dot(Vector3.up, p.transform.InverseTransformPoint(a.transform.position)) < 0;
    }

    public void FireProjectile()
    {
       
        if (!GameState.IsPaused)
        {
            if (IsInFront(player.gameObject, gameObject))
            {
                if (missileCounter > 0 && canShoot)
                { 
                    StartCoroutine(Missile(5));
                    missileCounter--;
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
        GameObject missile = Instantiate(missilePrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(time);
        canShoot = true;
        
    }

    private void MoveMissile()
    {
        GameObject[] missiles = GameObject.FindGameObjectsWithTag("MissileAI");
        foreach (GameObject missile in missiles)
        {
            missile.transform.LookAt(player.transform.position);
            missile.transform.Rotate(new Vector3(0, -90, 0), Space.Self);

            if (Vector3.Distance(missile.transform.position, player.transform.position) > 1)
            {
                missile.transform.Translate(new Vector3(45 * Time.deltaTime, 0, 0));
            }
            else
            {
                StartCoroutine(ReduceSpeed(3));
                Destroy(missile);
            }
        }
    }

    IEnumerator ReduceSpeed(float time)
    {
        player.Acceleration -= 25;
        yield return new WaitForSeconds(time);
        player.Acceleration = player.initialSpeed;
    }
}
