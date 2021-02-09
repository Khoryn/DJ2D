using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tire : MonoBehaviour
{
    [SerializeField]
    private float intensityModifier = 1.5f;

    private int lastSkidId = -1;

    ParticleSystem particles;

    // References
    SkidMarks skidMarksController;
    CarController carController;
    Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        skidMarksController = FindObjectOfType<SkidMarks>();
        carController = GetComponentInParent<CarController>();
        particles = GetComponent<ParticleSystem>();
    }

    private void LateUpdate()
    {
        if (GameState.IsPlaying) // If the race has begun
        {
            float intensity = Mathf.Abs(carController.DriftForce); // Get the player's current drift force, which is based on it's direction

            if (intensity < 0)
            {
                intensity = -intensity; // Reduce intensity
            }

            if (intensity > 3f)
            {
                player.Score += 10; // Increase player's score based on it's drift
                lastSkidId = skidMarksController.AddSkidMark(transform.position, transform.up, intensity * intensityModifier, lastSkidId); // Add skid marks on the tire's position

                if (particles != null && !particles.isPlaying)
                {
                    particles.Play(); // Play the dust particle
                }
            }
            else
            {
                lastSkidId = -1; // Set the id to -1 in order to stop the skid marks

                if (particles != null && particles.isPlaying)
                {
                    particles.Stop(); // Stop playing the particle system
                }
            }
        }
        else
        {
            particles.Stop(); // Stop playing the particle system
        }
    }
}
