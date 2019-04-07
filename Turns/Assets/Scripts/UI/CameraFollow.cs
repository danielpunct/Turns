using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gamelogic.Extensions;
using UnityEngine;

public class CameraFollow : Singleton<CameraFollow>
{
    public Transform gamePivot;
    public Transform menuPivot;
    public Transform skinsPivot;
    public Transform cam;
    
    public float followMultiplier = 4;
    public Transform player;

    Sequence seq;
    
    void FixedUpdate()
    {
        if (Game.Instance.IsStarted)
        {
            transform.position = Vector3.Lerp(transform.position, player.position, Runner.Instance.State != Runner.RunnerState.Cinematic ? Time.fixedDeltaTime * followMultiplier : 1);
        }
    }

    public void SetForMenu()
    {
        seq?.Kill();
        seq = DOTween.Sequence()
            .Insert(0, transform.DOMove(player.position, Runner.Instance.playerPresentOffset).SetEase(Ease.OutExpo))
            .Insert(0, cam.DOLocalMove(menuPivot.localPosition, 0.5f).SetEase(Ease.OutBack));
    }

    public void SetForGame()
    {
        seq?.Kill();
        seq = DOTween.Sequence()
            .Insert(0, cam.DOLocalMove(gamePivot.localPosition, 0.5f).SetEase(Ease.OutBack));
    }

    public void SetForSkins()
    {
        seq?.Kill();
        seq = DOTween.Sequence()
            .Insert(0, cam.DOLocalMove(skinsPivot.localPosition, 0.5f).SetEase(Ease.OutBack));
    }
}