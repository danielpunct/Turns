using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class FloorTile : MonoBehaviour
{
    public Vector3Int PositionKey;
    public Vector3Int? NextPositionKey;


    float appearDuration => Game.Instance.TilePassTime * 1.5f;
   

     public void Dissapear()
    {
        DOTween.Sequence()
            .Append(transform.DOScale(0, appearDuration))
            .AppendCallback(() => { PoolManager.Instance.TilesPool.Despawn(gameObject); });
    }

    public void Appear()
    {
        NextPositionKey = null;
        transform.localScale = Vector3.zero;
        transform.DOScale(1, appearDuration);
    }
}