using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GuiManager : MonoBehaviour
{
    [Header("Dialogue")]
    public GameObject dialogueContainer;
    [HideInInspector]
    public GameObject dialogueButton;

    [Header("Main Menu")]
    public GameObject mainMenu;
    public GameObject mainMenuButton;
    public GameObject mainMenuScene;

    [Header("Main Game")]
    public GameObject environment;
    public GameObject parallax;
    // Controllers
    private int Xbox_One_Controller = 0;
    private int PS4_Controller = 0;

    // Current Button Selected
    [HideInInspector]
    public GameObject currentSelected;

    // Script References
    Sound sound;
    Player player;

    void Start()
    {
        //Script references
        sound = FindObjectOfType<Sound>();
        player = FindObjectOfType<Player>();

        currentSelected = new GameObject();

        // Disable the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initial container state
        dialogueContainer.SetActive(false);

        // Set menu's initial state
        mainMenu.SetActive(true);
        MainMenu();
    }

    private void Update()
    {
        CloseMainMenu();

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(currentSelected);
        }
        else
        {
            currentSelected = EventSystem.current.currentSelectedGameObject;
        }
        CheckConnectedJoystick();
    }

    private void CheckConnectedJoystick()
    {
        string[] names = Input.GetJoystickNames();

        for (int x = 0; x < names.Length; x++)
        {
            if (names[x].Length == 19)
            {
                player.debugText.text = "PS4 CONTROLLER IS CONNECTED";
                Debug.Log("PS4 CONTROLLER IS CONNECTED");
                PS4_Controller = 1;
                Xbox_One_Controller = 0;
            }
            else if (names[x].Length == 33)
            {
                player.debugText.text = "XBOX ONE CONTROLLER IS CONNECTED";
                Debug.Log("XBOX ONE CONTROLLER IS CONNECTED");
                //set a controller bool to true
                PS4_Controller = 0;
                Xbox_One_Controller = 1;
            }
        }

        if (Xbox_One_Controller == 1)
        {
            // Change the icons for xbox
        }
        else if (PS4_Controller == 1)
        {
            // Change the icons for ps
        }
        else
        {
            // Change the icons for keyboard
        }

    }

    public IEnumerator ToggleDialogueContainerStart(float distance, float distanceToNpc)
    {
        if (distance < distanceToNpc && GameState.IsStartDialogue)
        {
            dialogueContainer.SetActive(true);

            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            dialogueContainer.SetActive(false);
        }
    }

    public void SetDialogueButton(GameObject button)
    {
        if (GameState.IsTalking)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(button);
        }
    }

    private void MainMenu()
    {
        if (mainMenu.activeInHierarchy)
        {
            // Play Main Menu Music
            sound.PlayMainMenuMusic(true);

            // Clear selected object
            EventSystem.current.SetSelectedGameObject(null);

            // Set the game state to paused
            GameState.ChangeState(GameState.States.Pause);

            // Set new selected object
            EventSystem.current.SetSelectedGameObject(mainMenuButton);

            mainMenuScene.SetActive(true);
            environment.SetActive(false);
            parallax.SetActive(false);
        }
    }

    private void CloseMainMenu()
    {
        if (!mainMenu.activeInHierarchy)
        {
            sound.PlayMainMenuMusic(false);
            mainMenuScene.SetActive(false);
            environment.SetActive(true);
            parallax.SetActive(true);
        }
    }

    public void StartGame()
    {
        if (mainMenu.activeInHierarchy)
        {
            mainMenu.SetActive(false);
            GameState.ChangeState(GameState.States.Idle);
        }
    }

    public void CatchMouseClicks(GameObject setSelection)
    {
        EventSystem.current.SetSelectedGameObject(setSelection);
    }
}

public class UIIgnoreRaycast : MonoBehaviour, ICanvasRaycastFilter
{
    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        return false;
    }
}
