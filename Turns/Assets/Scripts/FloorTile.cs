using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class FloorTile : MonoBehaviour
{
    public Vector3Int PositionKey { get; private set; }
    public Vector3Int? NextPositionKey;
    public bool IsHole { get; private set; }
    public MeshRenderer mesh;
    public Collider coll;


    float appearDuration => Game.Instance.TilePassTime * 1.5f;
   

     public void Dissapear()
    {
        DOTween.Sequence()
            .Append(transform.DOScale(0, appearDuration))
            .AppendCallback(() => { PoolManager.Instance.TilesPool.Despawn(gameObject); });
    }

    public void Appear(bool isHole, Vector3Int atPosition)
    {
        PositionKey = atPosition;
        IsHole = isHole;
    
        NextPositionKey = null;

        mesh.enabled = !isHole;
        coll.enabled = true;
        
        if (!isHole)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(1, appearDuration);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    public void LetPlayerFall()
    {
        coll.enabled = false;
    }
}