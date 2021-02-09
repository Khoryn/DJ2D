using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackLapTrigger : MonoBehaviour
{
    // next trigger in the lap
    public TrackLapTrigger next;

    // when an object enters this trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        CarLapCounter carLapCounter = other.gameObject.GetComponent<CarLapCounter>();
        if (carLapCounter)
        {
            carLapCounter.OnLapTrigger(this); // When the player enters, set the next trigger
        }
    }
}
