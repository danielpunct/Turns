using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        if (!Player.Instance.IsFalling)
        {
            transform.position = player.position;
        }
    }
}
