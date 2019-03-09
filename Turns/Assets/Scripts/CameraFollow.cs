using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        if (Game.Instance.GameStarted)
        {
            transform.position = player.position;
        }
    }
}
