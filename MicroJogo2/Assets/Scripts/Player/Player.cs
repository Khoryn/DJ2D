using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum State { IDLE, MOVE, ATTACK, DIALOGUE, DEAD }

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField]
    private float minJumpHeight = 4;
    [SerializeField]
    private float maxJumpHeight = 4;
    [SerializeField]
    private float timeToJumpApex = .4f;

    [Header("Movement Settings")]
    [SerializeField]
    private float initialMoveSpeed = 9;
    [SerializeField]
    private float boostedMoveSpeed = 13;
    private float moveSpeed;
    private float accelerationTimeAirborne = .2f;
    private float accelerationTimeGrounded = .1f;
    Vector2 input;

    private float minJumpVelocity;
    private float maxJumpVelocity;
    private float gravity;
    private float velocityXSmoothing;
    private Vector2 velocity;

    [Header("Wall Jump Settings")]
    public float wallSlideSpeedMax = 3;
    public float wallStickTime = 0.25f;
    private float timeToWallUnstick;
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    [Header("Light Settings")]
    [SerializeField]
    private HardLight2D playerLight;

    Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;

    [Header("NPC Dialogue")]
    public List<DialogueTrigger> npcList = new List<DialogueTrigger>();
    [HideInInspector]
    public float distanceToNPC;

    [Header("Stamina Bar")]
    public GameObject staminaBar;
    public float currentSprint, maxSprint;

    private Animator anim;

    Controller2D controller;
    DialogueManager dialogueManager;

    GUIManager guiManager;

    [HideInInspector]
    public State state;

    private void Start()
    {
        // Initial Player State
        state = State.IDLE;

        // Script references
        dialogueManager = FindObjectOfType<DialogueManager>();
        guiManager = FindObjectOfType<GUIManager>();
        controller = GetComponent<Controller2D>();

        // Gravity and jump velocity initial values;
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        // Animator
        anim = GetComponent<Animator>();

        //Light
        playerLight.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Synchronize all transform physics
        Physics2D.SyncTransforms();

        // Player Movement
        Movement();
        PlayerIncreasedSpeed();

        // Dialogue
        StartCoroutine(Dialogue());
        guiManager.ToggleDialogueTextStart(distanceToNPC);

        // Player Death
        DeathOnCollision();

        // Animations
        RunAnimation();
        JumpAnimation();
        FallingAnimation();

        // Light
        TurnLightOnAndOff();

        // Stamina Bar
        //StaminaBar();
    }

    #region Movement
    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (wallSliding)
        {
            #region Unused Wall Jump
            //if (wallDirX == input.x)
            //{
            //    velocity.x = -wallDirX * wallJumpClimb.x;
            //    velocity.y = wallJumpClimb.y;
            //}
            //else if (directionalInput.x == 0)
            //{
            //    velocity.x = -wallDirX * wallJumpOff.x;
            //    velocity.y = wallJumpOff.y;
            //}
            //else
            //{
            //    velocity.x = -wallDirX * wallLeap.x;
            //    velocity.y = wallLeap.y;
            //}
            #endregion

            if ((directionalInput.x > 0 && directionalInput.y >= 0f) && controller.collisions.left || (directionalInput.x < 0 && directionalInput.y > 0f) && controller.collisions.right)
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }

        if (controller.collisions.below && state != State.DIALOGUE)
        {
            velocity.y = maxJumpVelocity;
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    private void Movement()
    {
        if (state != State.DIALOGUE)
        {
            CalculateVelocity();
            HandleWallSliding();

            controller.Move(velocity * Time.deltaTime, input);

            if (controller.collisions.above || controller.collisions.below)
            {
                velocity.y = 0;
            }
        }
    }

    private void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;

        wallSliding = false;

        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }
            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = velocity.x = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }
    #endregion

    #region Animations and dialogue
  
    public DialogueTrigger GetClosestNPC(List<DialogueTrigger> npcs, Transform fromThis)
    {
        DialogueTrigger nearestTarget = null;
        distanceToNPC = 3;
        Vector3 currentPosition = fromThis.position;
        foreach (DialogueTrigger potentialTarget in npcs)
        {
            Vector2 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < distanceToNPC)
            {
                distanceToNPC = dSqrToTarget;
                nearestTarget = potentialTarget;
            }
        }
        return nearestTarget;
    }

    private IEnumerator Dialogue()
    {
        // if distance to NPC is smaller than something and Input.GetButtonDown("Fire2") the dialogue will begin
        if (GetClosestNPC(npcList, transform) != null)
        {
            if (Input.GetButtonDown("Circle") && Vector2.Distance(transform.position, GetClosestNPC(npcList, transform).transform.position) < 3 && state != State.DIALOGUE)
            {
                AnimTrigger("Idle");
                GetClosestNPC(npcList, transform).TriggerDialogue();
                yield return new WaitForSeconds(0.1f);
                state = State.DIALOGUE;
                // Camera Zoom!
            }

            if (Input.GetButtonDown("Circle") && state == State.DIALOGUE)
            {
                dialogueManager.DisplayNextSentence();
            }
        }
    }

    private void DeathOnCollision()
    {
        if (controller.playerDeath)
        {
            Debug.Log("Dead");
        }
    }

    private void PlayerIncreasedSpeed()
    {
        if (Input.GetButton("L2") && controller.collisions.below && currentSprint <= maxSprint)
        {
            moveSpeed = boostedMoveSpeed;
            if (currentSprint < maxSprint)
            {
                currentSprint -= Time.deltaTime;
            }
        }
        else
        {
            moveSpeed = initialMoveSpeed;

            if (currentSprint <= maxSprint)
            {
                currentSprint += Time.deltaTime;
            }
        }
    }

    private void StaminaBar()
    {
        staminaBar.transform.localScale = new Vector3(currentSprint, 1, 1);
    }

    private void AnimTrigger(string triggerName)
    {
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                anim.ResetTrigger(param.name);
            }
        }
        anim.SetTrigger(triggerName);
    }

    private void RunAnimation()
    {
        if (state != State.DEAD)
        {
            if (velocity.x > 0f && state == State.IDLE || state == State.MOVE)
            {
                AnimTrigger("Run");
                transform.GetComponent<SpriteRenderer>().flipX = false;
            }
            else if (velocity.x < 0f && state == State.IDLE || state == State.MOVE)
            {
                AnimTrigger("Run");
                transform.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                AnimTrigger("Idle");
            }
        }
    }

    private void JumpAnimation()
    {
        if (velocity.y > 0)
        {
            AnimTrigger("Jump");
        }
    }

    private void FallingAnimation()
    {
        if (velocity.y < 0 && !controller.collisions.below && state != State.DEAD)
        {
            AnimTrigger("Fall");
        }
    }
    #endregion

    #region Lights

    private void TurnLightOnAndOff()
    {
        if (Input.GetButtonDown("Triangle"))
        {
            if (!playerLight.gameObject.activeInHierarchy)
            {
                playerLight.gameObject.SetActive(true);
            }
            else
            {
                playerLight.gameObject.SetActive(false);
            }
            Debug.Log("Light");
        }
    }

    #endregion
}

