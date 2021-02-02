using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour
{
    [Header("Text fields")]
    [SerializeField]
    private Text playerVelocityText;

    [SerializeField]
    private Text playerScoreText;

    [SerializeField]
    private Text rankText;

    [SerializeField]
    private Text timeText;

    [SerializeField]
    private Text countDownText;

    [SerializeField]
    private Text timeIsUp;

    [SerializeField]
    private GameObject mainMenu;

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
    CarController playerCar;
    AICarMovement ai;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        playerCar = FindObjectOfType<CarController>();
        ai = FindObjectOfType<AICarMovement>();

        mainMenu.SetActive(true);

        startingTimer = timer;
        startingCountDown = countDown;
    }

    private void Update()
    {
        DisplayVelocity();
        PlayTime();
        Ranking();

        if (!mainMenu.activeInHierarchy)
        {
            Time.timeScale = 1;
            StartGameCountdown();
        }
        else
        {
            Time.timeScale = 0;
        }

        playerScoreText.text = "Score " + player.score; 
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
        //// Reset positions
        //playerCar.transform.position = playerCar.startPosition;
        //player.transform.Rotate(new Vector3(0, 0, 0));
        //ai.transform.position = ai.startPosition;
        //ai.transform.Rotate(new Vector3(0, 0, 0));

        //// Reset timers
        //timer = startingTimer;
        //countDown = startingCountDown;

        SceneManager.LoadScene("MainScene");
    }

    private void DisplayVelocity()
    {
        playerVelocityText.text = $"{player.CurrentVelocityinKms()} km/h";
    }

    private void PlayTime()
    {
        if (!GameState.IsPaused)
        {
            timer -= Time.deltaTime;
            timeText.text = "Time " + Mathf.Round(timer);
            if (timer <= 0)
            {
                GameState.ChangeState(GameState.States.Pause);
                timer = 0;
                timeIsUp.gameObject.SetActive(true);
                returnButton.gameObject.SetActive(true);
            }
        }

        if (timer > 0)
        {
            returnButton.gameObject.SetActive(false);
            timeIsUp.gameObject.SetActive(false);
        }
    }

    private void StartGameCountdown()
    {
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

        if (IsInFront(player, ai))
        {
            rankText.text = "Rank 1";
        }
        else
        {
            rankText.text = "Rank 2";
        }
    }

    private bool IsInFront(GameObject p, GameObject a)
    {
        return Vector3.Dot(Vector3.up, p.transform.InverseTransformPoint(a.transform.position)) < 0;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
