using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GUIManager : MonoBehaviour
{
    public GameObject dialogueTextStart;

    [Header("Main Menu")]
    public GameObject mainMenu;

    [Header("Pause Menu")]
    public GameObject pauseGameMenu;

    [Header("Pause Settings Menu")]
    public GameObject settingsMenu;

    [Header("Main Settings Menu")]
    public GameObject mainMenuSettings;

    [Header("Menu First Button Selected")]
    public GameObject mainMenuButton;
    public GameObject pauseMenuButton;
    public GameObject pauseSettingsOpenMenuButton;
    public GameObject pauseSettingsClosedMenuButton;
    public GameObject mainSettingsOpenMenuButton;
    public GameObject mainSettingsClosedMenuButton;

    [Header("Canvas Group")]
    public CanvasGroup canvasGroup;

    [Header("Death")]
    public Image deathPanel;

    // Current Button Selected
    private GameObject currentSelected;

    Player player;
    PlayerInput playerInput;

    private void Start()
    {
        currentSelected = new GameObject();

        // Disable the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Script References
        player = FindObjectOfType<Player>();
        playerInput = FindObjectOfType<PlayerInput>();

        // Set Dialogue window to false on start
        dialogueTextStart.gameObject.SetActive(false);

        // Set menu's initial state
        mainMenu.SetActive(true);
        pauseGameMenu.SetActive(false);
        settingsMenu.SetActive(false);

        // Death Panel
        deathPanel.gameObject.SetActive(false);

        MainMenu();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Options") && !deathPanel.gameObject.activeInHierarchy)
        {
            PauseGame();

            if (settingsMenu.activeInHierarchy)
            {
                settingsMenu.SetActive(false);
            }
        }

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(currentSelected);
        }
        else
        {
            currentSelected = EventSystem.current.currentSelectedGameObject;
        }
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

    public void OpenMainMenu()
    {
        mainMenu.SetActive(true);

        // Clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        // Set new selected object
        EventSystem.current.SetSelectedGameObject(mainMenuButton);

    }

    private void MainMenu()
    {
        if (mainMenu.activeInHierarchy)
        {
            // Clear selected object
            EventSystem.current.SetSelectedGameObject(null);

            // Set new selected object
            EventSystem.current.SetSelectedGameObject(mainMenuButton);
        }
    }

    public void OpenPauseSettings()
    {
        settingsMenu.SetActive(true);

        // Clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        // Set new selected object
        EventSystem.current.SetSelectedGameObject(pauseSettingsOpenMenuButton);
    }

    public void ClosePauseSettings()
    {
        settingsMenu.SetActive(false);

        // Clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        // Set new selected object
        EventSystem.current.SetSelectedGameObject(pauseSettingsClosedMenuButton);
    }

    public void OpenMainMenuSettings()
    {
        mainMenuSettings.SetActive(true);

        // Clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        // Set new selected object
        EventSystem.current.SetSelectedGameObject(mainSettingsOpenMenuButton);
    }

    public void CloseMainMenuSettings()
    {
        mainMenuSettings.SetActive(false);

        // Clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        // Set new selected object
        EventSystem.current.SetSelectedGameObject(mainSettingsClosedMenuButton);
    }

    private void PauseGame()
    {
        if (!pauseGameMenu.activeInHierarchy)
        {
            Time.timeScale = 0;
            pauseGameMenu.SetActive(true);

            // Clear selected object
            EventSystem.current.SetSelectedGameObject(null);

            // Set new selected object
            EventSystem.current.SetSelectedGameObject(pauseMenuButton);
        }
        else
        {
            Time.timeScale = 1;
            pauseGameMenu.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        pauseGameMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void StartGame()
    {
        pauseGameMenu.SetActive(false);

        foreach (Transform child in mainMenu.transform)
        {
            child.gameObject.SetActive(false);
        }

        StartCoroutine(Fade(4));
        Time.timeScale = 1;

        // Reset Values
        player.transform.position = player.initialPosition;
    }

    public IEnumerator Fade(float time)
    {
        mainMenu.SetActive(false);
        deathPanel.gameObject.SetActive(true);
        for (float t = 0.01f; t < time;)
        {
            t += Time.deltaTime;
            t = Mathf.Min(t, time);
            canvasGroup.alpha = Mathf.Lerp(1, 0, Mathf.Min(1, t / time));
            yield return null;
        }
       
        // Resets
        yield return new WaitForSeconds(1f);
        
        pauseGameMenu.SetActive(false);
        deathPanel.gameObject.SetActive(false);
        canvasGroup.alpha = 1;

        foreach (Transform child in mainMenu.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
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
