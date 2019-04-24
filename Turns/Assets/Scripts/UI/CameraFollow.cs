using DG.Tweening;
using Gamelogic.Extensions;
using UnityEngine;

public class CameraFollow : Singleton<CameraFollow>
{
    public Transform gamePivot;
    public Transform menuPivot;
    public Transform skinsPivot;   
    public Transform holderLeftRotationPivot;
    public Transform holderBackRotationPivot;
    public GameObject endLevelPortalHolder;
    public GameObject endLevelConfettiHolder;

    public Transform cam;
    
    public float followMultiplier = 4;
    public Transform player;

    Sequence _seq;
    Transform _tr;
    bool endEffectDisplayed = false;

    void Awake()
    {
        _tr = transform;
    }

    void FixedUpdate()
    {
        if (Game.Instance.IsStarted)
        {
            transform.position = Vector3.Lerp(transform.position, player.position,
                Runner.Instance.State != Runner.RunnerState.Cinematic ? Time.fixedDeltaTime * followMultiplier : 1);

            if (Runner.Instance.IsWinWalking)
            {
                if (!endEffectDisplayed)
                {
                    endLevelPortalHolder.SetActive(true);
                    endEffectDisplayed = true;
                    _seq?.Kill();
                    _seq = DOTween.Sequence()
                        .Insert(0, _tr.DORotateQuaternion(FloorManager.Instance.CurrentDirection == VectorInt.back
                            ? holderBackRotationPivot.rotation
                            : holderLeftRotationPivot.rotation, 1f))
                        .Insert(1, endLevelPortalHolder.transform.DOScale(1, 0.4f).SetEase(Ease.OutBack))
                        .Insert(1.4f, endLevelPortalHolder.transform.DOScale(2, 10f))
                        // come back to play camera
                        .InsertCallback(1.5f, () => Game.Instance.OnRunnerJumpToWarp());
                }
            }
        }
    }

    public void ShowConfetti()
    {
        endLevelConfettiHolder.SetActive(true);
    }

    void Reset()
    {
        _tr.DORotateQuaternion(Quaternion.identity, 0.5f);
        endEffectDisplayed = false;
        endLevelPortalHolder.SetActive(false);
        endLevelConfettiHolder.SetActive(false);
        endLevelPortalHolder.transform.localScale = Vector3.zero;
    }

    public void SetForMenu()
    {
        _seq?.Kill();
        _seq = DOTween.Sequence()
            .Insert(0, transform.DOMove(player.position, Runner.Instance.playerPresentOffset).SetEase(Ease.OutExpo))
            .Insert(0, cam.DOLocalMove(menuPivot.localPosition, 0.5f).SetEase(Ease.OutBack));
        Reset();
    }

    public void SetForGame()
    {
        _seq?.Kill();
        _seq = DOTween.Sequence()
            .Insert(0, cam.DOLocalMove(gamePivot.localPosition, 0.5f).SetEase(Ease.OutBack));
        Reset();
    }

    public void SetForSkins()
    {
        _seq?.Kill();
        _seq = DOTween.Sequence()
            .Insert(0, cam.DOLocalMove(skinsPivot.localPosition, 0.5f).SetEase(Ease.OutBack));
        Reset();
    }
}