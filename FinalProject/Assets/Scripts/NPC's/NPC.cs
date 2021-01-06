using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private float distanceToPlayer;
    private Player player;

    [Header("Friendship")]
    public float friendshipLevel;

    [Header("Movement")]
    public float speed;

    // Script Reference
    DialogueParser dialogue;

    void Start()
    {
        dialogue = GetComponent<DialogueParser>();
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        StartDialogueWithPlayer();
    }

    private void StartDialogueWithPlayer()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < distanceToPlayer)
        {
            if (GameState.IsStartDialogue)
            {
                dialogue.ProceedToNarrative(dialogue.narrativeData.TargetNodeGUID);
                GameState.ChangeState(GameState.States.Talking);
            }
        }
    }
}
