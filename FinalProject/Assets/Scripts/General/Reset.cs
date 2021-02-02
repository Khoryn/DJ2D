using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    Player player;
    Aze aze;
    Sound sound;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        aze = FindObjectOfType<Aze>();
        sound = FindObjectOfType<Sound>();
    }

    public void ResetGeneralGame()
    {

    }

    public void ResetPlayer()
    {
        player.transform.position = player.initialPosition;
        player.CurrentCheckPoint(0);
    }

    public void ResetDeath()
    {
        aze.transform.position = aze.GetComponent<NPC>().initialPosition;
        aze.gameObject.SetActive(true);
        aze.GetComponent<NPC>().friendshipLevel = aze.GetComponent<NPC>().initialFriendshipLevel;
        sound.hasPlayed = false;
    }

    public void ResetElrynd()
    {

    }

    public void ResetValindra()
    {

    }
}
