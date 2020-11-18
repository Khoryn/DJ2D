using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    Player player;
    GUIManager guiManager;

    [HideInInspector]
    public Vector2 directionalInput;

    void Start()
    {
        player = GetComponent<Player>();
        guiManager = FindObjectOfType<GUIManager>();
    }

    void Update()
    {
        if (!guiManager.pauseGameMenu.activeInHierarchy && !guiManager.mainMenu.activeInHierarchy && !guiManager.deathPanel.gameObject.activeInHierarchy)
        {
            directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            player.SetDirectionalInput(directionalInput);

            if (Input.GetButtonDown("Cross") || Input.GetKeyDown(KeyCode.Space))
            {
                player.OnJumpInputDown();
            }

            if (Input.GetButtonUp("Cross") || Input.GetKeyUp(KeyCode.Space))
            {
                player.OnJumpInputUp();
            }
        }
    }
}
