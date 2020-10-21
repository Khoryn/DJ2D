using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEncounterGenerator : MonoBehaviour
{
    Player player;
    BattleSystem battleSystem;

    // This is the starting probability of an encounter
    const int DEFAULT_ENCOUNTER_THRESHOLD = 10;

    // Set the current probability to the default value
    private int currentEncounterThreshold = DEFAULT_ENCOUNTER_THRESHOLD;

    private Vector2 oldPosition;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        battleSystem = FindObjectOfType<BattleSystem>();

        oldPosition = player.transform.position;
    }

    private void Update()
    {
        RandomEncounter();
    }

    public void StartBattleWithPlayer()
    {
        // Begin the battle
        StartCoroutine(battleSystem.SetupBattle());
        // Set the player's state to COMBAT
        player.state = PlayerState.COMBAT;
    }

    public void RandomEncounter()
    {
        if (oldPosition != new Vector2(player.transform.position.x, player.transform.position.y) && player.state != PlayerState.COMBAT) // Chance for random encounter if player has moved
        {
            // Pick a number between 0 and 100
            int value = Random.Range(0, 1000000);

            // Check if the number is below the current threshold
            if (value < currentEncounterThreshold)
            {
                // If it is, then start an encounter, and set the threshold back to the default value for next time.
                StartBattleWithPlayer();
                currentEncounterThreshold = DEFAULT_ENCOUNTER_THRESHOLD;
            }
            else
            {
                // We weren't below the threshold this time, so let's increase it
                currentEncounterThreshold += 1;
            }
            oldPosition = player.transform.position;

            //Debug.Log("The current value is " + value);
        }
    }
}


