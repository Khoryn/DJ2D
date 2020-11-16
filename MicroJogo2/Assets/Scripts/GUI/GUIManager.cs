using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public GameObject dialogueTextStart;

    public GameObject pauseGameMenu;
    private bool pauseGame;

    Player player;

    private void Start()
    {
        Cursor.visible = false;
        player = FindObjectOfType<Player>();
        dialogueTextStart.gameObject.SetActive(false);
    }

    private void Update()
    {
        PauseGameMenu();
    }

    public void ToggleDialogueTextStart(float distance)
    {
        if (distance < 3 && player.state != State.DIALOGUE)
        {
            dialogueTextStart.gameObject.SetActive(true);
        }
        else
        {
            dialogueTextStart.gameObject.SetActive(false);
        }
    }

    private void PauseGameMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseGame = !pauseGame;
        }

        if (pauseGame)
        {
            Time.timeScale = 0;
            // Show Pause Menu
        }
        else
        {
            Time.timeScale = 1;
            // Hide Pause Menu
        }
    }
}
