using DG.Tweening;
using Gamelogic.Extensions;
using UnityEngine;

public class CameraFollow : Singleton<CameraFollow>
{
    public Transform cam;
    public Transform gamePivot;
    public Transform menuPivot;
    public Transform skinsPivot;   
    public Transform holderLeftRotationPivot;
    public Transform holderBackRotationPivot;
    public float followMultiplier = 4;
    public Transform player;
    [Header("End level effects")]
    public Transform endLevelEffectsOrientedHolder;
    public GameObject endLevelPortalHolder;
    public ParticleSystem endLevelConfettiHolder;
    public AnimationCurve portalScaleCurve;

    Sequence _seq;
    Sequence _seqReset;
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
                        .Insert(1, endLevelPortalHolder.transform.DOScale(3f, 8f).SetEase(portalScaleCurve))
                        .InsertCallback(2f, () => Game.Instance.OnRunnerJumpToWarp());
                }
            }
        }
    }

    public void ShowConfetti()
    {
        endLevelConfettiHolder.Play();
    }

    void Reset(bool continueLevel)
    {
        _tr.DORotateQuaternion(Quaternion.identity, 1f);
        if (continueLevel)
        {
            _seqReset?.Kill();
            _seqReset = DOTween.Sequence()
                .Insert(0, endLevelEffectsOrientedHolder.DOLocalMoveZ(-15, 0.4f).SetEase(Ease.InCubic))
                .InsertCallback(1, () =>
                {
                    endEffectDisplayed = false;
                    endLevelPortalHolder.SetActive(false);
                    endLevelPortalHolder.transform.localScale = Vector3.zero;
                    endLevelEffectsOrientedHolder.localPosition = Vector3.zero;
                });
        }
        else
        {
            endEffectDisplayed = false;
            endLevelPortalHolder.SetActive(false);
            endLevelPortalHolder.transform.localScale = Vector3.zero;
            endLevelEffectsOrientedHolder.localPosition = Vector3.zero;
        }
    }

    public void SetForMenu()
    {
        _seq?.Kill();
        _seq = DOTween.Sequence()
            .Insert(0, transform.DOMove(player.position, Runner.Instance.playerPresentOffset).SetEase(Ease.OutExpo))
            .Insert(0, cam.DOLocalMove(menuPivot.localPosition, 0.5f).SetEase(Ease.OutBack));
        Reset(false);
    }

    public void SetForGame(bool continueLevel)
    {
        _seq?.Kill();
        _seq = DOTween.Sequence()
            .Insert(0, cam.DOLocalMove(gamePivot.localPosition, 0.5f).SetEase(Ease.OutBack));
        Reset(continueLevel);
    }

    public void SetForSkins()
    {
        _seq?.Kill();
        _seq = DOTween.Sequence()
            .Insert(0, cam.DOLocalMove(skinsPivot.localPosition, 0.5f).SetEase(Ease.OutBack));
        Reset(false);
    }
}