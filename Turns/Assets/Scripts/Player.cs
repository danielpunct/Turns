using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector3 direction = Vector3.forward;
    private Rigidbody rb;
    private Transform tr;
    private Vector3? currentTilePosition = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tr = transform;
    }

    private void FixedUpdate()
    {
        if (!Game.Instance.GameStarted)
        {
            return;
        }

        rb.MovePosition( tr.localPosition + direction * Speed());
        
        CheckTilePosition();
    }

    float Speed()
    {
        return 1 / Game.Instance.InitialTilePassTime * Time.fixedDeltaTime;
    }

    void CheckTilePosition()
    {
        var pos = tr.localPosition;

        var tilePosition = new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));

        if (currentTilePosition != null && tilePosition != currentTilePosition)
        {
            FloorManager.Instance.OnPlayerPassedTile();
        }

        currentTilePosition = tilePosition;
    }
}
