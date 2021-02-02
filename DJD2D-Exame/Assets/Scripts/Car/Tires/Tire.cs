using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tire : MonoBehaviour
{
    [SerializeField] float intensityModifier = 1.5f;
    SkidMarks skidMarksController;
    CarController carController;
    Player player;

    ParticleSystem particles;

    int lastSkidId = -1;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        skidMarksController = FindObjectOfType<SkidMarks>();
        carController = GetComponentInParent<CarController>();
        particles = GetComponent<ParticleSystem>();
    }

    private void LateUpdate()
    {
        if (GameState.IsPlaying)
        {
            float intensity = Mathf.Abs(carController.DriftForce);

            if (intensity < 0)
            {
                intensity = -intensity;
            }

            if (intensity > 3f)
            {
                player.score += 10;
                lastSkidId = skidMarksController.AddSkidMark(transform.position, transform.up, intensity * intensityModifier, lastSkidId);

                if (particles != null && !particles.isPlaying)
                {
                    particles.Play();
                }
            }
            else
            {
                lastSkidId = -1;

                if (particles != null && particles.isPlaying)
                {
                    particles.Stop();
                }
            }

            Debug.Log(intensity);
        }
        else
        {
            particles.Stop();
        }

    }
}
