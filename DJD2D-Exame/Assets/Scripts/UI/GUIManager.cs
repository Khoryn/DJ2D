using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour
{
    [Header("Text field - Player")]
    [SerializeField]
    private Text playerVelocityText;

    [SerializeField]
    private Text playerScoreText;

    [SerializeField]
    private Text rankText;

    [Header("Text field - Time")]
    [SerializeField]
    private Text timeText;

    [SerializeField]
    private Text countDownText;

    [SerializeField]
    private Text timeIsUpText;

    [Header("Text field - Missiles")]
    [SerializeField]
    private Text missileCounterText;

    [Header("Main Menu")]
    [SerializeField]
    private GameObject mainMenu;

    [Header("Return Button")]
    [SerializeField]
    private Button returnButton;

    [Header("Timer")]
    [SerializeField]
    private float timer;
    private float startingTimer;

    [Header("Countdown")]
    [SerializeField]
    private float countDown;
    private float startingCountDown;

    [Header("Buttons")]
    [SerializeField]
    private Button startGameButton;

    // Script References
    Player player;
    AICarMovement ai;
    CarLapCounter carLapCounter;

    private void Start()
    {
        // Script References
        player = FindObjectOfType<Player>();
        ai = FindObjectOfType<AICarMovement>();
        carLapCounter = FindObjectOfType<CarLapCounter>();

        // Activate main menu on game start
        mainMenu.SetActive(true);

        // Set initial values
        startingTimer = timer;
        startingCountDown = countDown;
    }

    private void Update()
    {
        DisplayVelocity();
        PlayTime();
        Ranking();
        DisplayMissileCounter();

        if (!mainMenu.activeInHierarchy)
        {
            Time.timeScale = 1;
            StartGameCountdown();
        }
        else
        {
            Time.timeScale = 0;
        }
        playerScoreText.text = "Score " + player.Score; 
    }

    public void StartGame()
    {
        if (GameState.IsPaused)
        {
            mainMenu.SetActive(false);
            timer = startingTimer;
            countDown = startingCountDown;
            startGameButton.gameObject.SetActive(false);
        }
    }

    public void RestartGame()
    {
        // Reset scene
        SceneManager.LoadScene("MainScene");
    }

    private void DisplayVelocity()
    {
        // Display player's velocity in km's / h
        playerVelocityText.text = $"{player.CurrentVelocityinKms()} km/h";
    }

    private void DisplayMissileCounter()
    {
        // Display player's missile counter
        missileCounterText.text = "Missiles " + player.MissileCounter;
    }
   
    private void PlayTime()
    {
        if (!GameState.IsPaused)
        {
            // Start game timer
            timer -= Time.deltaTime;
            timeText.text = "Time " + Mathf.Round(timer); // Round up the game timer
            if (timer <= 0)
            {
                GameState.ChangeState(GameState.States.Pause); // Set the game state to Paused if the timer reaches 0
                timer = 0;
                timeIsUpText.gameObject.SetActive(true);
                returnButton.gameObject.SetActive(true);
            }
        }

        if (timer > 0)
        {
            returnButton.gameObject.SetActive(false);
            timeIsUpText.gameObject.SetActive(false);
        }
    }

    private void StartGameCountdown()
    {
        // When the game starts, set the race countdown
        countDown -= Time.deltaTime;
        countDownText.text = Mathf.Round(countDown).ToString();
        if (countDown <= 0 && timer > 0)
        {
            countDownText.gameObject.SetActive(false);
            GameState.ChangeState(GameState.States.Playing);
            Debug.Log("Start race!");
        }
    }

    public void Ranking()
    {
        GameObject player = GameObject.Find("PlayerCar");

        GameObject ai = GameObject.Find("AI");

        if (IsInFront(player, ai)) // If the player is in front of the AI, set it's rank to 1
        {
            rankText.text = "Rank 1";
        }
        else // If the player is in behind of the AI, set it's rank to 2
        {
            rankText.text = "Rank 2";
        }
    }

    private bool IsInFront(GameObject player, GameObject ai)
    {
        // Check if the player is in front or behind the AI, needs work
        return Vector3.Dot(Vector3.up, player.transform.InverseTransformPoint(ai.transform.position)) < 0;
    }

    public void QuitGame()
    {
        // Quit the game
        Application.Quit();
    }
}
