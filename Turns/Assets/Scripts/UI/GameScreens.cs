using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameScreens : MonoBehaviour
{
    public enum GameState
    {
        play,
        levelTransition,
    };
    
    public Progress progressUI;
    public ComboUI comboUI;
    
    Sequence _menuSeq;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(GameState state)
    {
        UpdateState();
        
        gameObject.SetActive(true);
        
        _menuSeq?.Kill();
        _menuSeq = DOTween.Sequence();

        switch (state)
        {
            case GameState.play:
                _menuSeq
                    .Insert(0, progressUI.transform.DOLocalMoveY(0, 0));
                MomentsRecorderHelper.Instance.ResetRecording();
                break;
            case GameState.levelTransition:
                _menuSeq
                    .Insert(0, progressUI.transform.DOLocalMoveY(500, 1));
                break;
        }
        
        
        comboUI.Hide();
    }

    public void UpdateState()
    {
        progressUI.UpdateState();
    }

    public void DisplayComboPoints(int points)
    {
        comboUI.Show($"+{points}");
    }
}
