using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    public void Reset()
    {
        transform.position = player.position;
    }
    
    void Update()
    {
        if (Game.Instance.GameStarted && !Player.Instance.IsCurrentlyDieing)
        {
            transform.position = player.position;
        }
    }
}
