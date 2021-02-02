using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private float distanceToPlayer;
    private float currentDistanceToPlayer;
    private Player player;

    [Header("Friendship")]
    [HideInInspector]
    public float friendshipLevel;

    [Header("Movement")]
    public float speed;

    [Header("Initial Settings")]
    public Vector2 initialPosition;
    public float initialFriendshipLevel;

    // Script Reference
    DialogueParser dialogue;
    GuiManager gui;

    void Start()
    {
        dialogue = GetComponent<DialogueParser>();
        player = FindObjectOfType<Player>();
        gui = FindObjectOfType<GuiManager>();

        // Set initial NPC friendship level
        friendshipLevel = initialFriendshipLevel;

        // Set initial NPC position
        transform.position = initialPosition;
    }

    void Update()
    {
        currentDistanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        gui.ToggleDialogueTextStart(currentDistanceToPlayer);

        StartDialogueWithPlayer();
    }

    private void StartDialogueWithPlayer()
    {
        if (currentDistanceToPlayer < distanceToPlayer)
        {
            if (GameState.IsStartDialogue && !dialogue.hasTalked)
            {
                dialogue.ProceedToNarrative(dialogue.narrativeData.TargetNodeGUID);
                GameState.ChangeState(GameState.States.Talking);
            }
        }
    }
}
