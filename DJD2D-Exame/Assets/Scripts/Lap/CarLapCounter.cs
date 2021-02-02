using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarLapCounter : MonoBehaviour
{
    [Header("First Trigger")]
    public TrackLapTrigger first;

    public Text lapCounter;
    public Text lapTime;

    TrackLapTrigger next;
    int _lap;
    float time;
    float minutes;
    float seconds;
    float milliseconds;

    bool startCounter = false;

    public float LapTime { get { return time; } }

    [HideInInspector]
    public float bestTime;

    private GameObject skidMarks;

    // Use this for initialization
    void Start()
    {
        skidMarks = GameObject.Find("Game Manager");
        lapTime.gameObject.SetActive(false);

        _lap = 1;
        SetNextTrigger(first);
        UpdateText();
    }

    private void Update()
    {
        Timer();
    }

    // update lap counter text
    void UpdateText()
    {
        if (lapCounter)
        {
            lapCounter.text = string.Format("Lap {0}", _lap);
        }
    }

    private void Timer()
    {
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
        lapTime.gameObject.SetActive(true);
        lapTime.text = string.Format("{0}:{1}:{2}", minutes, seconds, (int)milliseconds);
        StartCoroutine(ResetTimer(0.05f));
        yield return new WaitForSeconds(time);
        lapTime.gameObject.SetActive(false);
    }

    IEnumerator ResetTimer(float time)
    {
        startCounter = false; 
        yield return new WaitForSeconds(time);
        startCounter = true;
    }

    // when lap trigger is entered
    public void OnLapTrigger(TrackLapTrigger trigger)
    {
        if (trigger == next)
        {
            if (first == next)
            {
                time = 0;
                _lap++;
                UpdateText();
                StartCoroutine(ShowLapTime(2));
                if (_lap == 1)
                {
                    bestTime = time;
                }
              
                BestLapTime();

            }
            SetNextTrigger(next);
        }

        if (trigger == first)
        {
            startCounter = true;
        }
    }

    private void BestLapTime()
    {
        if (_lap > 1)
        {
            if (time < bestTime)
            {
                bestTime = time;
            }
        }
       

        Debug.Log(bestTime);
    }

    void SetNextTrigger(TrackLapTrigger trigger)
    {
        next = trigger.next;
        SendMessage("OnNextTrigger", next, SendMessageOptions.DontRequireReceiver);
    }
}
