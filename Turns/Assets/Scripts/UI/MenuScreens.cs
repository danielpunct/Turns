using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreens : MonoBehaviour
{
    public enum MenuState
    {
        init,
        home,
        continueRun,
        gameOver,
        skins,
    };
    
    public GameObject defaultHolder;
    public GameObject gameOverHolder;
    public GameObject continueHolder;
    [Header("default")]
    public CanvasGroup lateHolder;
    public MenuButtonsUI buttonsUI;
    public TMP_Text titleText;
    public GameObject elementsSkins;
    public Skins skins;
    public GameObject skinButtonsHolder;
    [Header("game over")]
    public TMP_Text bestRunText;
    public TMP_Text yourRunText;
    public Transform replayHolder;
    [Header("continue")] 
    public Image timerBorder;
    public Transform continueButtonHolder;
    public Transform continueTextHolder;

    Sequence _menuSeq;
    Sequence _skinsSeq;
    Sequence _continueSeq;

    
    public void Show(MenuState state)
    {
        gameObject.SetActive(true);
        
        _menuSeq?.Kill();
        _menuSeq = DOTween.Sequence();
            
        defaultHolder.SetActive(state == MenuState.init || state == MenuState.home);
        gameOverHolder.SetActive(state == MenuState.gameOver);
        continueHolder.SetActive(state == MenuState.continueRun);
        
        skins.transform.DOLocalMoveX(1400, 0);
        elementsSkins.SetActive(false);
        
        titleText.DOFade(0, 0);
        skinButtonsHolder.SetActive(false);
        lateHolder.gameObject.SetActive(true);
        lateHolder.alpha = 0;
        _menuSeq.Insert(0.5f, lateHolder.DOFade(1, 1f));
        
        switch (state)
        {
            case MenuState.init:
                _continueSeq?.Kill();
                buttonsUI._Reset();
                buttonsUI.SwitchButtons(_menuSeq, 1, true, false);
                _menuSeq
                    .Insert(1, titleText.DOFade(1, 3))
                    .InsertCallback(Runner.Instance.playerPresentOffset, () => { skinButtonsHolder.SetActive(true); });
                break;
            case MenuState.continueRun:
                buttonsUI._Reset();

                continueButtonHolder.localScale = Vector3.zero;
                continueTextHolder.localScale  = Vector3.one;
                
                _menuSeq
                    .InsertCallback(0.5f, () => MomentsRecorderHelper.Instance.CaptureReplay())
                    .Insert(0, continueButtonHolder.DOScale(1, 0.8f) )
                    .Insert(0.5f, continueTextHolder.DOScale(1.15f, 0.5f).SetLoops(2, LoopType.Yoyo));
                StartContinueTimer();
                break;
            case MenuState.gameOver:
                _continueSeq?.Kill();
                buttonsUI._Reset();
                buttonsUI.SwitchButtons(_menuSeq, 1, true, false);
                bestRunText.text = "Best Score\n" + GameManager.Instance.Player.MaxPoints;
                yourRunText.text =  GameManager.Instance.Player.LastPoints.ToString();

                if (Game.Instance.SkipContinue)
                {
                    _menuSeq
                        .InsertCallback(0.5f, () =>
                        {
                            MomentsRecorderHelper.Instance.CaptureReplay();
                            FloorManager.Instance._Reset();
                        });
                }
                else
                {
                    FloorManager.Instance._Reset();
                }

                _menuSeq
                    .InsertCallback(0.5f, () => MomentsRecorderHelper.Instance.StartPlayback())
                    .Insert(2f, replayHolder.DOPunchRotation(new Vector3(0, 0, 22), 0.8f, 10, 1));
                break;
            case MenuState.home:
                _menuSeq
                    .Insert(1, titleText.DOFade(1, 3))
                    .InsertCallback(Runner.Instance.playerPresentOffset, () => { skinButtonsHolder.SetActive(true); });
                break;
            case MenuState.skins:
                break;
        }
        
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
        lateHolder.DOFade(0, 0.4f).OnComplete(() => { lateHolder.gameObject.SetActive(false); });

        _continueSeq?.Kill();
    }
    
    public void OnSwitchSkinsButtonClick(bool on)
    {
        if (on)
        {
            Show(MenuState.skins);
            elementsSkins.SetActive(true);
            buttonsUI.SwitchButtons(_skinsSeq, 0, false, true);
            CameraFollow.Instance.SetForSkins();
        }
        else
        {
            Show(MenuState.init);
            elementsSkins.SetActive(false);
            buttonsUI.SwitchButtons(_menuSeq, 3, true, false);
            CameraFollow.Instance.SetForMenu();
        }
        
        _skinsSeq?.Kill();
        _skinsSeq = DOTween.Sequence()
            .Insert(0, skins.transform.DOLocalMoveX(on ? 0 : 1400, 0.1f));
        
    }

    void StartContinueTimer()
    {
        var duration = 5;
        timerBorder.fillAmount = 1;
        _continueSeq?.Kill();
        _continueSeq = DOTween.Sequence()
            .Insert(0, timerBorder.DOFillAmount(0, duration).SetEase(Ease.Linear))
            .InsertCallback(duration, () => Menu.Instance.OnNoThanksClick());
    }
    
}