using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarLapCounter : MonoBehaviour
{
    [Header("First Trigger")]
    [SerializeField]
    private TrackLapTrigger first;

    [Header("UI Text")]
    [SerializeField]
    private Text lapCounter;
    [SerializeField]
    private Text lapTime;

    private int lap;
    private float minutes;
    private float seconds;
    private float milliseconds;

    private bool startCounter = false;

    private float time;
    public float LapTime { get { return time; } }

    private float bestTime;
    public float BestTime { get { return bestTime; } }

    private GameObject skidMarks;

    TrackLapTrigger next;

    // Use this for initialization
    void Start()
    {
        skidMarks = GameObject.Find("Game Manager");
        lapTime.gameObject.SetActive(false);

        lap = 1;
        SetNextTrigger(first);
        UpdateText();
    }

    private void Update()
    {
        Timer();
    }

    // Update lap counter text
    void UpdateText()
    {
        if (lapCounter)
        {
            lapCounter.text = string.Format("Lap {0}", lap);
        }
    }

    private void Timer()
    {
        // Set the lap time based on minutes, seconds and milliseconds
        if (startCounter)
        {
            time += Time.deltaTime;
            minutes = Mathf.Floor(time / 60);
            seconds = Mathf.RoundToInt(time % 60);
            milliseconds = (int)(time * 1000f) % 1000;
        }
    }

    IEnumerator ShowLapTime(float time)
    {
        // Show lap time after the player enter's the Start/Finish line
        lapTime.gameObject.SetActive(true);
        lapTime.text = string.Format("{0}:{1}:{2}", minutes, seconds, (int)milliseconds);
        StartCoroutine(ResetTimer(0.05f));
        yield return new WaitForSeconds(time);
        lapTime.gameObject.SetActive(false);
    }

    IEnumerator ResetTimer(float time)
    {
        // Reset the timer after the player enters the Start / Finish line
        startCounter = false; 
        yield return new WaitForSeconds(time);
        startCounter = true;
    }

    // When enter lap trigger
    public void OnLapTrigger(TrackLapTrigger trigger)
    {
        if (trigger == next)
        {
            if (first == next) // If the player is back on the initial trigger
            {
                time = 0; // Reset lap time
                lap++; // Increment lap
                UpdateText(); 
                StartCoroutine(ShowLapTime(2)); // Show lap time
                if (lap == 1)
                {
                    bestTime = time;
                }   
                BestLapTime();
            }
            SetNextTrigger(next); // Set the next trigger
        }

        if (trigger == first)
        {
            startCounter = true;
        }
    }

    private void BestLapTime()
    {
        if (lap > 1)
        {
            if (time < bestTime)
            {
                bestTime = time;
            }
        }
    }

    void SetNextTrigger(TrackLapTrigger trigger)
    {
        next = trigger.next;
        SendMessage("OnNextTrigger", next, SendMessageOptions.DontRequireReceiver);
    }
}
