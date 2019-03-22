using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gamelogic.Extensions;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    Transform tr;
    float rotattion = 0.01f;
    void Awake()
    {
        tr = transform;
    }

    // Update is called once per frame
    void Update()
    {
        tr.Rotate(rotattion, 0, 0, Space.Self);
    }
}
