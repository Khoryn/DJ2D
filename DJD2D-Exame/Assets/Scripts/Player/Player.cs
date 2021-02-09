using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int score = 0;

    public int Score
    {
        get { return score; }
        set { score = value; }
    }

    [SerializeField]
    private int missileCounter;

    public int MissileCounter { get { return missileCounter; } }

    [SerializeField]
    private GameObject missilePrefab;

    AICarMovement ai;

    private void Start()
    {
        ai = FindObjectOfType<AICarMovement>();

        // Set initial game state
        GameState.ChangeState(GameState.States.Pause);
    }

    private void Update()
    {
        FireProjectile();
        MoveMissile();
    }

    public float CurrentVelocityinKms()
    {
        // Convert rigidbody velocity to km's /h 
        return (float)Math.Round(GetComponent<Rigidbody2D>().velocity.magnitude * 3.6f, 0);
    }

    public void FireProjectile()
    {
        if (!GameState.IsPaused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (missileCounter > 0) // Fire missile if counter is greater than 0
                {
                    missileCounter--; // Reduce missile counter
                    Missile(); // Fire missile
                    Debug.Log("Fire Missile");
                }
                else
                {
                    Debug.Log("Out of missiles");
                }
            }
        }
    }

    private void Missile()
    {
        GameObject missile = Instantiate(missilePrefab, transform.position, Quaternion.identity); // Instantiate missile at player's position
    }

    private void MoveMissile()
    {
        GameObject[] missiles = GameObject.FindGameObjectsWithTag("Missile");
        foreach (GameObject missile in missiles)
        {
            missile.transform.LookAt(ai.transform.position); // Rotate missile to it looks at ai transform
            missile.transform.Rotate(new Vector3(0, -90, 0), Space.Self);

            if (Vector3.Distance(missile.transform.position, ai.transform.position) > 1)
            {
                missile.transform.Translate(new Vector3(45 * Time.deltaTime, 0, 0)); // Move missile while it's distance to AI is greater than 1
            }
            else
            {
                StartCoroutine(ReduceSpeed(3)); // After the missile hits the AI, reduce the AI's speed temporarily
                Destroy(missile);
                score += 500; // When the missile hits the AI, increase score by 500
            }
        }
    }

    IEnumerator ReduceSpeed(float time)
    {
        ai.MaxSpeed -= 20; // Reduce the AI's speed by 20
        yield return new WaitForSeconds(time);
        ai.MaxSpeed = ai.InitialSpeed; // Reset the AI's speed
    }
}
