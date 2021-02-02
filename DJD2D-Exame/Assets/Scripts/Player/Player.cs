using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int score = 0;

    public int missileCounter;

    public GameObject missilePrefab;

    public Text missileCounterText;


    AICarMovement ai;
    private void Start()
    {
        ai = FindObjectOfType<AICarMovement>();

        GameState.ChangeState(GameState.States.Pause);     
    }

    private void Update()
    {
        //Debug.Log(GameState.state);

        FireProjectile();
        MoveMissile();

        missileCounterText.text = "Missiles " + missileCounter;
    }

    public float CurrentVelocityinKms()
    {
        return (float)Math.Round(GetComponent<Rigidbody2D>().velocity.magnitude * 3.6f, 0);
    }

    public void FireProjectile()
    {
        if (!GameState.IsPaused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (missileCounter > 0)
                {
                    missileCounter--;
                    Missile();
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
        GameObject missile = Instantiate(missilePrefab, transform.position, Quaternion.identity);
    }

    private void MoveMissile()
    {
        GameObject[] missiles = GameObject.FindGameObjectsWithTag("Missile");
        foreach (GameObject missile in missiles)
        {
            missile.transform.LookAt(ai.transform.position);
            missile.transform.Rotate(new Vector3(0, -90, 0), Space.Self);

            if (Vector3.Distance(missile.transform.position, ai.transform.position) > 1)
            {
                missile.transform.Translate(new Vector3(45 * Time.deltaTime, 0, 0));
            }
            else
            {
                StartCoroutine(ReduceSpeed(3));
                Destroy(missile);
                score += 500;
            }
        }
    }

    IEnumerator ReduceSpeed(float time)
    {
        ai.maxSpeed -= 20;
        yield return new WaitForSeconds(time);
        ai.maxSpeed = ai.initialSpeed;
    }
}
