using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GuiManager : MonoBehaviour
{
    [Header("Canvas Group")]
    public CanvasGroup canvasGroup;

    [Header("Death")]
    public Image deathPanel;

    [Header("Dialogue")]
    public GameObject dialogueContainer;
    [HideInInspector]
    public GameObject dialogueButton;
    public GameObject dialogueTextStart;

    [Header("Main Menu")]
    public GameObject mainMenu;
    public GameObject mainMenuButton;
    public GameObject mainMenuScene;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public GameObject pauseMenuButton;

    [Header("Settings Menu Main")]
    public GameObject settingsMenuMain;
    public GameObject settingsMenuButtonMain;

    [Header("Settings Menu Pause")]
    public GameObject settingsMenuPause;
    public GameObject settingsMenuButtonPause;

    [Header("Credits Screen")]
    public GameObject creditsScreen;
    public GameObject creditsSceneButton;

    [Header("Main Game")]
    public GameObject environment;
    public GameObject parallax;

    [Header("Game Start Text")]
    public GameObject gameStartText;

    [Header("Sound sliders Main Menu")]
    public Slider masterSlider;
    public Slider effectsSlider;
    public Slider musicSlider;
    public Slider ambientSlider;

    [Header("Sound sliders Pause Menu")]
    public Slider masterSliderPause;
    public Slider effectsSliderPause;
    public Slider musicSliderPause;
    public Slider ambientSliderPause;


    // Controllers
    private int Xbox_One_Controller = 0;
    private int PS4_Controller = 0;

    // Current Button Selected
    [HideInInspector]
    private GameObject currentSelected;

    // Script References
    Sound sound;
    Player player;
    Reset reset;

    void Start()
    {
        //Script references
        sound = FindObjectOfType<Sound>();
        player = FindObjectOfType<Player>();
        reset = FindObjectOfType<Reset>();

        currentSelected = new GameObject();

        // Disable the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initial container state
        dialogueContainer.SetActive(false);

        // Set menu's initial state
        mainMenu.SetActive(true);
        MainMenu();
        gameStartText.SetActive(false);

        // Set pause menu initial state
        pauseMenu.SetActive(false);

        // Set pause menu initial state
        creditsScreen.SetActive(false);

        // Set settings menu initial state
        settingsMenuMain.SetActive(false);
        settingsMenuPause.SetActive(false);
    }

    private void Update()
    {
        PauseMenu();
        CheckActiveMenus();

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(currentSelected);
        }
        else
        {
            currentSelected = EventSystem.current.currentSelectedGameObject;
        }

        if (mainMenu.activeInHierarchy)
        {
            player.GetComponent<SpriteRenderer>().enabled = false;
            Time.timeScale = 1;
        }
        else
        {
            player.GetComponent<SpriteRenderer>().enabled = true;
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

    #region Main Menu
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
            player.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            mainMenu.SetActive(false);
            player.GetComponent<SpriteRenderer>().enabled = false;
            Time.timeScale = 1;
        }
    }

    public void NewGame()
    {
        //CloseMainMenu();

        if (mainMenu.activeInHierarchy)
        {
            mainMenu.SetActive(false);
            sound.PlayMainMenuMusic(false);
            mainMenuScene.SetActive(false);
            environment.SetActive(true);
            parallax.SetActive(true);
            gameStartText.SetActive(true);

            player.GetComponent<SpriteRenderer>().enabled = true;

            GameState.ChangeState(GameState.States.Idle);
            Time.timeScale = 1;
            reset.ResetPlayer();
            reset.ResetDeath();
            StartCoroutine(Fade(8));
        }
    }
    #endregion

    #region Pause Menu
    private void PauseMenu()
    {
        if (Input.GetButtonDown("Options") && !mainMenu.activeInHierarchy && !creditsScreen.activeInHierarchy && !settingsMenuPause.activeInHierarchy && !settingsMenuMain.activeInHierarchy && !gameStartText.activeInHierarchy)
        {
            if (!pauseMenu.activeInHierarchy)
            {
                Time.timeScale = 0;
                pauseMenu.SetActive(true);

                // Clear selected object
                EventSystem.current.SetSelectedGameObject(null);

                // Set the game state to paused
                GameState.ChangeState(GameState.States.Pause);

                // Set new selected object
                EventSystem.current.SetSelectedGameObject(pauseMenuButton);
            }
            else
            {
                Time.timeScale = 1;
                pauseMenu.SetActive(false);
                GameState.ChangeState(GameState.States.Idle);
            }
        }
    }

    public void ResumeGameFromPauseMenu()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        GameState.ChangeState(GameState.States.Idle);
    }

    public void SaveGame()
    {

    }

    public void SettingsMenuFromPauseMenu()
    {

    }

    public void MainMenuFromPauseMenu()
    {
        if (!mainMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
            mainMenu.SetActive(true);

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

    #endregion

    #region Credits Scene
    public void OpenCreditsScreen()
    {
        if (!creditsScreen.activeInHierarchy)
        {
            CloseMainMenu();
            creditsScreen.SetActive(true);

            // Clear selected object
            EventSystem.current.SetSelectedGameObject(null);

            // Set new selected object
            EventSystem.current.SetSelectedGameObject(creditsSceneButton);
        }
    }

    public void MainMenuFromCredits()
    {
        if (!mainMenu.activeInHierarchy)
        {
            creditsScreen.SetActive(false);
            mainMenu.SetActive(true);

            // Clear selected object
            EventSystem.current.SetSelectedGameObject(null);

            // Set new selected object
            EventSystem.current.SetSelectedGameObject(mainMenuButton);

            mainMenuScene.SetActive(true);
        }
    }
    #endregion

    #region Settings Menu

    public void OpenSettingsMenuFromMainMenu()
    {
        if (!settingsMenuMain.activeInHierarchy)
        {
            CloseMainMenu();
            settingsMenuMain.SetActive(true);

            // Clear selected object
            EventSystem.current.SetSelectedGameObject(null);

            // Set new selected object
            EventSystem.current.SetSelectedGameObject(settingsMenuButtonMain);

            masterSlider.value = PlayerPrefs.GetFloat("Master", 0.75f);
            effectsSlider.value = PlayerPrefs.GetFloat("Effects", 0.75f);
            musicSlider.value = PlayerPrefs.GetFloat("Music", 0.75f);
            ambientSlider.value = PlayerPrefs.GetFloat("Ambient", 0.75f);
        }
    }

    public void OpenSettingsMenuFromPauseMenu()
    {
        if (!settingsMenuPause.activeInHierarchy)
        {
            //CloseMainMenu();
            settingsMenuPause.SetActive(true);
            pauseMenu.SetActive(false);

            // Clear selected object
            EventSystem.current.SetSelectedGameObject(null);

            // Set new selected object
            EventSystem.current.SetSelectedGameObject(settingsMenuButtonPause);

            masterSliderPause.value = PlayerPrefs.GetFloat("Master", 0.75f);
            effectsSliderPause.value = PlayerPrefs.GetFloat("Effects", 0.75f);
            musicSliderPause.value = PlayerPrefs.GetFloat("Music", 0.75f);
            ambientSliderPause.value = PlayerPrefs.GetFloat("Ambient", 0.75f);
        }
    }

    public void CloseSettingsMenuToMainMenu()
    {
        if (!mainMenu.activeInHierarchy)
        {
            settingsMenuMain.SetActive(false);
            mainMenu.SetActive(true);

            // Clear selected object
            EventSystem.current.SetSelectedGameObject(null);

            // Set new selected object
            EventSystem.current.SetSelectedGameObject(mainMenuButton);

            mainMenuScene.SetActive(true);
        }
    }

    public void CloseSettingsMenuToPauseMenu()
    {
        if (!pauseMenu.activeInHierarchy)
        {
            settingsMenuPause.SetActive(false);
            pauseMenu.SetActive(true);
            // Clear selected object
            EventSystem.current.SetSelectedGameObject(null);

            // Set new selected object
            EventSystem.current.SetSelectedGameObject(pauseMenuButton);
        }
    }

    #endregion

    #region Active Menus

    private void CheckActiveMenus()
    {
        if (creditsScreen.activeInHierarchy)
        {
           
        }
    }

    #endregion

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ToggleDialogueTextStart(float distance)
    {
        if (distance < 2 && !GameState.IsTalking && !GameState.IsPaused)
        {
            dialogueTextStart.gameObject.SetActive(true);
        }
        else
        {
            dialogueTextStart.gameObject.SetActive(false);
        }
    }

    public void CatchMouseClicks(GameObject setSelection)
    {
        EventSystem.current.SetSelectedGameObject(setSelection);
    }

    public IEnumerator Fade(float time)
    {
        mainMenu.SetActive(false);
        deathPanel.gameObject.SetActive(true);
        GameState.ChangeState(GameState.States.Pause);
        for (float t = 0.01f; t < time;)
        {
            t += Time.deltaTime;
            t = Mathf.Min(t, time);
            canvasGroup.alpha = Mathf.Lerp(1, 0, Mathf.Min(1, t / time));
            yield return null;
        }

        // Resets
        yield return new WaitForSeconds(0.00001f);
        GameState.ChangeState(GameState.States.Idle);
        gameStartText.SetActive(false);
        pauseMenu.SetActive(false);
        deathPanel.gameObject.SetActive(false);
        canvasGroup.alpha = 1;

        foreach (Transform child in mainMenu.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void CheckConnectedJoystick()
    {
        string[] names = Input.GetJoystickNames();

        for (int x = 0; x < names.Length; x++)
        {
            if (names[x].Length == 19)
            {
                Debug.Log("PS4 CONTROLLER IS CONNECTED");
                PS4_Controller = 1;
                Xbox_One_Controller = 0;
            }
            else if (names[x].Length == 33)
            {
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
}

public class UIIgnoreRaycast : MonoBehaviour, ICanvasRaycastFilter
{
    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        return false;
    }
}
