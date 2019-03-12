using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float followMultiplier = 4;
    public Transform player;

    void FixedUpdate()
    {
        if (Game.Instance.IsStarted)
        {
            transform.position = Vector3.Lerp(transform.position, player.position, Player.Instance.IsRunning ? Time.fixedDeltaTime * followMultiplier : 1);
        }
    }
}
