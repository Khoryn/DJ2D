using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    [Range(0.0f, 2.0f)]
    public float dampTime;
    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    public Vector3 mainMenuPosition;

    GuiManager gui;

    private void Start()
    {
        gui = FindObjectOfType<GuiManager>();
    }

    void Update()
    {
        if (gui.mainMenu.activeInHierarchy || gui.creditsScreen.activeInHierarchy || gui.settingsMenuMain.activeInHierarchy)
        {
            transform.position = mainMenuPosition;
        }
        else
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        if (target)
        {
            Vector3 point = Camera.main.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            Vector3 destination = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination + offset, ref velocity, dampTime);
        }
    }
}
