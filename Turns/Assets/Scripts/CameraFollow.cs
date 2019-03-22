using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gamelogic.Extensions;
using UnityEngine;

public class CameraFollow : Singleton<CameraFollow>
{
    public Transform gamePivot;
    public Transform menuPivot;
    public Transform cam;
    
    public float followMultiplier = 4;
    public Transform player;

    Tween cTween;
    
    void FixedUpdate()
    {
        if (Game.Instance.IsStarted)
        {
            transform.position = Vector3.Lerp(transform.position, player.position, Player.Instance.IsRunning ? Time.fixedDeltaTime * followMultiplier : 1);
        }
    }

    public void SetForMenu()
    {
        cTween?.Kill();
        transform.position = player.position;
//        transform.DOMove( player.position, 0.1f).SetEase(Ease.OutExpo);
        cTween = cam.DOLocalMove(menuPivot.localPosition, 0.5f).SetEase(Ease.OutBack);
    }

    public void SetForGame()
    {
        cTween?.Kill();
        
        cTween = cam.DOLocalMove(gamePivot.localPosition, 0.5f).SetEase(Ease.OutBack);
    }
}
