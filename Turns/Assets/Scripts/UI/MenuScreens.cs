using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MenuScreens : MonoBehaviour
{
    public enum MenuState
    {
        init,
        home,
        continueRun,
        gameOver
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

    Sequence _menuSeq;
    Sequence _skinsSeq;

    
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
                buttonsUI._Reset();
                buttonsUI.SwitchButtons(_menuSeq, 1, true, false);
                _menuSeq
                    .Insert(1, titleText.DOFade(1, 3))
                    .InsertCallback(Runner.Instance.playerPresentOffset, () => { skinButtonsHolder.SetActive(true); });
                break;
            case MenuState.continueRun:
                buttonsUI._Reset();
                _menuSeq
                    .InsertCallback(0.5f, () => MomentsRecorderHelper.Instance.CaptureReplay());
                break;
            case MenuState.gameOver:
                FloorManager.Instance._Reset();
                buttonsUI._Reset();
                buttonsUI.SwitchButtons(_menuSeq, 1, true, false);
                bestRunText.text = "Best Score\n" + GameManager.Instance.Player.MaxPoints;
                yourRunText.text =  GameManager.Instance.Player.LastPoints.ToString();
                _menuSeq
                    .InsertCallback(1f, () => MomentsRecorderHelper.Instance.StartPlayback());
                break;
            case MenuState.home:
                _menuSeq
                    .Insert(1, titleText.DOFade(1, 3))
                    .InsertCallback(Runner.Instance.playerPresentOffset, () => { skinButtonsHolder.SetActive(true); });
                break;
        }
        
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
        lateHolder.DOFade(0, 0.4f).OnComplete(() => { lateHolder.gameObject.SetActive(false); });

    }
    
    public void ShitchSkinsUI(bool on)
    {
        _skinsSeq?.Kill();
        _skinsSeq = DOTween.Sequence()
            .Insert(0, skins.transform.DOLocalMoveX(on ? 0 : 1400, 0.1f));

        if (on)
        {
            elementsSkins.SetActive(true);
//            elementsMain.SetActive(false);
            buttonsUI.SwitchButtons(_skinsSeq, 0, false, true);
            CameraFollow.Instance.SetForSkins();
        }
        else
        {
            elementsSkins.SetActive(false);
//            elementsMain.SetActive(true);
            buttonsUI.SwitchButtons(_menuSeq, 3, true, false);
            CameraFollow.Instance.SetForMenu();
        }
    }
    
}