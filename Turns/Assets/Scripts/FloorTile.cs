using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FloorTile : MonoBehaviour
{
    public float appearDuration = 0.4f;
   

    public void Dissapear()
    {
        DOTween.Sequence()
            .Append(transform.DOScale(0, appearDuration))
            .AppendCallback(() => { PoolManager.Instance.TilesPool.Despawn(gameObject); });
    }

    public void Appear()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, appearDuration);
    }
}