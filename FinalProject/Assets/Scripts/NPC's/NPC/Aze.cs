using System.Collections;
using System.Collections.Generic;
using Subtegral.DialogueSystem.DataContainers;
using UnityEngine;
using UnityEngine.UI;

public class Aze : MonoBehaviour
{
    public List<GameObject> checkpoints;
    public Transform checkpointContainer;
    public Transform checkPoint;
    private Animator anim;

    NPC npc;
    Player player;
    Sound sound;

    void Start()
    {
        // Script References
        player = FindObjectOfType<Player>();
        npc = GetComponent<NPC>();
        sound = FindObjectOfType<Sound>(); 

        anim = GetComponent<Animator>();

        FindCheckpoints(checkpointContainer, "NPC Checkpoint", checkpoints);
    }

    private void Update()
    {
        RunToCheckPointCave();

        DeathDialogue();
    }

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

    private void RunToCheckPointCave()
    {
        if (player.CurrentCheckPoint(1))
        {
            sound.ComePlayGameSound(5);
            transform.position = Vector2.MoveTowards(transform.position, checkpoints[0].transform.position, npc.speed * Time.deltaTime);
            anim.SetTrigger("Run");
            GetComponent<SpriteRenderer>().flipX = true;    
        }

        if (Vector2.Distance(transform.position, checkpoints[0].transform.position) < 2)
        {
            gameObject.SetActive(false);
        }
    }

    // Environment changes according to the this npc's friendship level
    private void DeathDialogue()
    {
        // Starting Dialogue
        if (npc.friendshipLevel == 50)
        {
            GetComponent<DialogueParser>().dialogue = (DialogueContainer)Resources.Load("test");
        }

        #region Reduced Friendship

        // Friendship has been reduced by a maximum of 20
        if (npc.friendshipLevel < 50 && npc.friendshipLevel > 30)
        {

        }

        // Friendship has been reduced even more
        if (npc.friendshipLevel < 30 && npc.friendshipLevel > 10)
        {

        }

        // The player won't be able to interact with this NPC any longer
        if (npc.friendshipLevel == 0)
        {

        }
        #endregion

        #region Increased Friendship

        // Friendship has been increased by a maximum of 20
        if (npc.friendshipLevel > 50 && npc.friendshipLevel < 70)
        {

        }

        // Friendship has been increased even more
        if (npc.friendshipLevel < 70 && npc.friendshipLevel > 90)
        {

        }

        // Is taken into account for one the possible endings
        if (npc.friendshipLevel == 100)
        {

        }
        #endregion
    }
}
