using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    void FixedUpdate()
    {
        if (!Player.Instance.IsFalling)
        {
            transform.position = Vector3.Lerp(transform.position, player.position, Time.fixedDeltaTime * 3);
        }
    }
}
