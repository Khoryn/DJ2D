using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Aze : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> checkpoints;
    [SerializeField]
    private Transform checkpointContainer;
    private Transform checkPoint;
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
}
