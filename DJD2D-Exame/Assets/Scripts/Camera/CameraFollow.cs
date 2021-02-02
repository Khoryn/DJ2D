using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    Vector3 offset;

    void Update()
    {
        CameraFollowPlayer();
    }

    private void CameraFollowPlayer()
    {
        transform.position = player.transform.position + offset;
    }
}
