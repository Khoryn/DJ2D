using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Vector2 input;

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

    [Header("Player Initial Settings")]
    public float distanceToCheckpoint = 3;
    public Transform checkpointContainer;
    public Vector2 initialPosition = new Vector2();
    [HideInInspector]
    public Vector2 playerCheckpoint;
    [HideInInspector]
    public List<GameObject> checkpoints;

    //[Header("Light Settings")]
    //[SerializeField]
    //private HardLight2D playerLight;

    Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;

    [Header("NPC Dialogue")]
    public List<NPC> npcList = new List<NPC>();
    [SerializeField]
    private float distanceToNpc = 3;
    private float distance;

    private Animator anim;

    Controller2D controller;
    GuiManager guiManager;

    //DialogueManager dialogueManager;

    // Debug
    public Text debugText;

    private void Start()
    {
        // Player initial position
        transform.position = initialPosition;

        // Set initial player movement speed
        moveSpeed = initialMoveSpeed;
        GravityAndJumpValues();

        // Script references   
        guiManager = FindObjectOfType<GuiManager>();
        controller = GetComponent<Controller2D>();
        //dialogueManager = FindObjectOfType<DialogueManager>();
        //camera = FindObjectOfType<CameraFollowPlayer>();

        // Animator
        anim = GetComponent<Animator>();

        // Find NPC's
        FindNpcs();

        // Find Checkpoints
        FindCheckpoints(checkpointContainer, "Checkpoint", checkpoints);
        playerCheckpoint = checkpointContainer.transform.GetChild(0).transform.position;

        //Light
        //playerLight.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Synchronize all transform physics
        Physics2D.SyncTransforms();

        if (!GameState.IsPaused)
        {
            // Player Movement
            Movement();
            MovementStates();

            // Dialogue
            Dialogue();

            // Player Death
            DeathOnCollision();

            // Animations
            RunAnimation();
            JumpAnimation();
            FallingAnimation();

            //Checkpoints
            SetNewPlayerCheckpoint(distanceToCheckpoint);
        }
    }

    #region Movement

    private void GravityAndJumpValues()
    {
        // Gravity and jump velocity initial values;
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

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

            //if ((directionalInput.x > 0 && directionalInput.y >= 0f) && controller.collisions.left || (directionalInput.x < 0 && directionalInput.y > 0f) && controller.collisions.right)
            //{
            //    velocity.x = -wallDirX * wallLeap.x;
            //    velocity.y = wallLeap.y;
            //}

            if ((directionalInput.x > 0 /*&& directionalInput.y >= 0f*/) && controller.collisions.left || (directionalInput.x < 0/* && directionalInput.y > 0f*/) && controller.collisions.right)
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }

        if (controller.collisions.below)
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
        if (!GameState.IsTalking)
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

    private void MovementStates()
    {
        if (!GameState.IsTalking)
        {
            if (velocity.x != 0)
            {
                GameState.ChangeState(GameState.States.Running);
            }
            else
            {
                GameState.ChangeState(GameState.States.Idle);
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

    private void FindNpcs()
    {
        foreach (NPC npc in Resources.FindObjectsOfTypeAll(typeof(NPC)))
        {
            npcList.Add(npc);
        }
    }

    public NPC GetClosestNPC(List<NPC> npcs, Transform fromThis)
    {
        NPC nearestTarget = null;
        distance = 5;
        Vector3 currentPosition = fromThis.position;
        foreach (NPC potentialTarget in npcs)
        {
            Vector2 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < distance)
            {
                distance = dSqrToTarget;
                nearestTarget = potentialTarget;
            }
        }
        return nearestTarget;
    }

    private void Dialogue()
    {
        // if distance to NPC is smaller than something and Input.GetButtonDown("Fire2") the dialogue will begin
        if (GetClosestNPC(npcList, transform) != null)
        {
            if (Input.GetButtonDown("Circle") && Vector2.Distance(transform.position, GetClosestNPC(npcList, transform).transform.position) < distanceToNpc && !GameState.IsTalking)
            {
                AnimTrigger("Idle");
                GameState.ChangeState(GameState.States.StartTalking);
                StartCoroutine(guiManager.ToggleDialogueContainerStart(distance, distanceToNpc));
            }
        }
    }

    private void DeathOnCollision()
    {
        if (controller.playerDeath)
        {
            controller.playerDeath = false;
            velocity.x = 0;
            transform.position = playerCheckpoint;
            StartCoroutine(guiManager.Fade(2f));
        }
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
        if (!GameState.IsDead)
        {
            if (velocity.x > 0f && GameState.IsRunning)
            {
                AnimTrigger("Run");
                transform.GetComponent<SpriteRenderer>().flipX = false;
            }
            else if (velocity.x < 0f && GameState.IsRunning)
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
            GameState.ChangeState(GameState.States.Jumping);
            AnimTrigger("Jump");
        }
    }

    private void FallingAnimation()
    {
        if (velocity.y < 0 && !controller.collisions.below && !GameState.IsDead)
        {
            GameState.ChangeState(GameState.States.Falling);
            AnimTrigger("Fall");
        }
    }

    #endregion

    #region Lights

    //private void TurnLightOnAndOff()
    //{
    //    if (Input.GetButtonDown("Triangle"))
    //    {
    //        if (!playerLight.gameObject.activeInHierarchy)
    //        {
    //            playerLight.gameObject.SetActive(true);
    //        }
    //        else
    //        {
    //            playerLight.gameObject.SetActive(false);
    //        }
    //        Debug.Log("Light");
    //    }
    //}

    #endregion

    #region Checkpoints

    private void FindCheckpoints(Transform parent, string tag, List<GameObject> list)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.tag == tag)
            {
                list.Add(child.gameObject);
            }
        }
    }

    private void SetNewPlayerCheckpoint(float distanceToCheckpoint)
    {
        Vector2 tempCheckpoint;
        foreach (GameObject checkpoint in checkpoints)
        {
            float distance = Vector2.Distance(transform.position, checkpoint.transform.position);

            if (distance < distanceToCheckpoint)
            {
                tempCheckpoint = checkpoint.transform.position;
                playerCheckpoint = tempCheckpoint;
            }
        }
    }

    public bool CurrentCheckPoint(int index)
    {
        if (playerCheckpoint == new Vector2(checkpoints[index].transform.position.x, checkpoints[index].transform.position.y))
        {
            return true;
        }
        return false;
    }
    #endregion
}
