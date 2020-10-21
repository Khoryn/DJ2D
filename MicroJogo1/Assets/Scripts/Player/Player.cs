using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public enum PlayerState { COMBAT, WALKING, DIALOGUE }
public class Player : MonoBehaviour
{
    [HideInInspector]
    public PlayerState state;
    private bool inCombat;

    [HideInInspector]
    public bool hasMoved;

    public float speed = 2f;

    private Sprite[] spriteAtlas;
    private Sprite playerSprite;

    private Rigidbody2D rb2d;

    #region Movement Buttons
    [HideInInspector]
    public Button buttonNorth;
    [HideInInspector]
    public Button buttonSouth;
    [HideInInspector]
    public Button buttonEast;
    [HideInInspector]
    public Button buttonWest;
    #endregion


    void Start()
    {
        // Load all sprites in atlas
        spriteAtlas = Resources.LoadAll<Sprite>("Wizard");

        // Starting player state
        state = PlayerState.WALKING;

        rb2d = GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 0;
    }

    private void Update()
    {
        MovePlayer();
    }

    public Sprite GetSpriteByName(string name)
    {
        for (int i = 0; i < spriteAtlas.Length; i++)
        {
            if (spriteAtlas[i].name == name)
            {
                return spriteAtlas[i];
            }
        }
        return null;
    }

    public void MovePlayer()
    {
        if (buttonNorth.GetComponent<ButtonPressed>().isPressed)
        {
            MovePlayerNorth();
        }
        else if (buttonSouth.GetComponent<ButtonPressed>().isPressed)
        {
            MovePlayerSouth();
        }
        else if (buttonEast.GetComponent<ButtonPressed>().isPressed)
        {
            MovePlayerEast();
        }
        else if(buttonWest.GetComponent<ButtonPressed>().isPressed)
        {
            MovePlayerWest();
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = GetSpriteByName("Wizard_0");
        }
    }

    public void MovePlayerNorth()
    {
        GetComponent<SpriteRenderer>().sprite = GetSpriteByName("Wizard_0");
        Vector3 move = new Vector3(0, 1, 0);
        transform.position += move * speed * Time.deltaTime;
    }

    public void MovePlayerSouth()
    {
        GetComponent<SpriteRenderer>().sprite = GetSpriteByName("Wizard_1");
        Vector3 move = new Vector3(0, -1, 0);
        transform.position += move * speed * Time.deltaTime;
    }

    public void MovePlayerEast()
    {
        GetComponent<SpriteRenderer>().sprite = GetSpriteByName("Wizard_3");
        Vector3 move = new Vector3(-1, 0, 0);
        transform.position += move * speed * Time.deltaTime;
    }

    public void MovePlayerWest()
    {
        GetComponent<SpriteRenderer>().sprite = GetSpriteByName("Wizard_2");
        Vector3 move = new Vector3(1, 0, 0);
        transform.position += move * speed * Time.deltaTime;
    }

    public void ResetPlayer()
    {
        transform.position = new Vector3(0, 0, 0);
        state = PlayerState.WALKING;
    }
}

public class DownstateButton : Button
{
    public void Update()
    {
        if (IsPressed())
        {
            WhilePressed();
        }
    }

    public void WhilePressed()
    {
        Debug.Log("IsPressed");
    }
}
