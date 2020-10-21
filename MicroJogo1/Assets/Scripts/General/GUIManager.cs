using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject deathPanel;
    public GameObject winPanel;
    public GameObject settingsMenu;
    public GameObject lights;

    private Color menuOgColor;

    Volume volume;
    BattleSystem system;

    private void Start()
    {
        menuPanel.SetActive(true);
        settingsMenu.SetActive(false);
        deathPanel.SetActive(false);
        winPanel.SetActive(false);
        lights.SetActive(false);

        menuOgColor = menuPanel.GetComponent<Image>().color;

        system = FindObjectOfType<BattleSystem>();
    }

    public void StartGame()
    {
        // Start Game
        StartCoroutine(FadeMainMenu(true, 1));
    }

    IEnumerator FadeMainMenu(bool fade, float time)
    {
        Transform[] ts = menuPanel.GetComponentsInChildren<Transform>();

        foreach (Transform child in menuPanel.transform)
        {
            child.gameObject.SetActive(false);
        }

        if (fade)
        {
            for (float i = time; i >= 0; i -= Time.deltaTime)
            {
                menuPanel.GetComponent<Image>().color = new Color(41, 36, 36, i);
                lights.SetActive(true);
                yield return null;
            }
            menuPanel.SetActive(false);
            menuPanel.GetComponent<Image>().color = menuOgColor;

            foreach (Transform child in menuPanel.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
    public void OpenSettingsMenu()
    {
        settingsMenu.gameObject.SetActive(true);
    }

    public void SoundSettings()
    {
        // Adjust volume settings
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SettingsMenuBack()
    {
        settingsMenu.gameObject.SetActive(false);
    }

    public void Restart()
    {
        deathPanel.SetActive(false);
        winPanel.SetActive(false);
        menuPanel.SetActive(true);
        //lights.SetActive(false);
    }

    public void Death()
    {
        deathPanel.SetActive(true);
        lights.SetActive(false);
    }

    public void WinGame()
    {
        winPanel.SetActive(true);
        lights.SetActive(false);
    }
}
