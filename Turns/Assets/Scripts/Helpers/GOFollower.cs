using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOFollower : MonoBehaviour
{
    public Transform transformToFollow;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = transformToFollow.position;
    }
}
