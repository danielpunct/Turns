using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gamelogic.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Menu : Singleton<Menu>
{

    [Header("Menu")] public GameObject menuUIHolder;
    public TMP_Text titleText;
    public CanvasGroup lateHolder;
    public GameObject elementsMain;
    public GameObject elementsSkins;
    public CanvasGroup buttonHolder1;
    public CanvasGroup buttonHolder2;
    public CanvasGroup buttonHolder3;
    public CanvasGroup buttonHolder4;
    public Skins skins;
    public GameObject skinButtonsHolder;

    [Header("Game")] public GameObject gameUIHolder;
    public Progress progress;

    Sequence _menuSeq;
    Sequence _skinsSeq;

    void Awake()
    {
        ResetMenuUI();
    }

    void ResetMenuUI()
    {
        elementsMain.SetActive(true);
        elementsSkins.SetActive(false);
        lateHolder.gameObject.SetActive(true);
        lateHolder.alpha = 0;
        buttonHolder1.alpha = 0;
        buttonHolder2.alpha = 0;
        buttonHolder3.alpha = 0;
        buttonHolder4.alpha = 0;

        buttonHolder1.transform.DOLocalMoveY(-40, 0);
        buttonHolder2.transform.DOLocalMoveY(-40, 0);
        buttonHolder3.transform.DOLocalMoveY(-40, 0);
        buttonHolder4.transform.DOLocalMoveY(-40, 0);
        skins.transform.DOLocalMoveX(1400, 0);
        titleText.DOFade(0, 0);
        
        skinButtonsHolder.SetActive(false);
    }

    void ResetElements()
    {
        Player.Instance.Reset(); // need to be done before camera
        CameraFollow.Instance.SetForMenu();
        FloorManager.Instance.Reset();
    }

    public void ShowMenu(bool init)
    {
        menuUIHolder.SetActive(true);
        gameUIHolder.SetActive(false);

        ResetMenuUI();

        _menuSeq?.Kill();
        _menuSeq = DOTween.Sequence()
            .Insert(init ? 1 : 3, titleText.DOFade(1, 3))
            .Insert(1, lateHolder.DOFade(1, 1f))
            .InsertCallback(init ? 0 : 2, ResetElements)
            .InsertCallback((init ? 0 : 3) + Player.Instance.playerPresentOffset, () => { skinButtonsHolder.SetActive(true); });

        switchButtons(_menuSeq, init ? 1 : 3, true, false);
    }

    public void ShowGameMenu()
    {
        menuUIHolder.SetActive(false);
        gameUIHolder.SetActive(true);
        lateHolder.DOFade(0, 0.4f).OnComplete(() => { lateHolder.gameObject.SetActive(false); });
    }

    public void OnPlayClick()
    {
        _menuSeq?.Kill();
        ResetElements();
        GameManager.Instance.StartAnotherGame();
    }


    public void UpdateUI()
    {
        if (Game.Instance.IsStarted)
        {
            progress.Display();
        }
    }


    public void ShitchSkinsUI(bool on)
    {
        _skinsSeq?.Kill();
        _skinsSeq = DOTween.Sequence()
            .Insert(0, skins.transform.DOLocalMoveX(on ? 0 : 1400, 0.1f));

        if (on)
        {
            elementsSkins.SetActive(true);
            elementsMain.SetActive(false);
            switchButtons(_skinsSeq, 0, false, true);
            CameraFollow.Instance.SetForSkins();
        }
        else
        {
            elementsSkins.SetActive(false);
            elementsMain.SetActive(true);
            switchButtons(_menuSeq, 3, true, false);
            CameraFollow.Instance.SetForMenu();
        }
    }

    void switchButtons(Sequence seq, float offset, bool on, bool farther)
    {
        const float buttonsDuration = 0.35f;
        var eachOffset = on ? 0.07f : 0;
        var fadeoffset = on ? 0 : buttonsDuration / 2f;

        seq.Insert(offset + fadeoffset + eachOffset * 0.5f * 0, buttonHolder1.DOFade(on ? 1 : 0, buttonsDuration))
            .Insert(offset + fadeoffset + eachOffset * 0.5f * 1, buttonHolder2.DOFade(on ? 1 : 0, buttonsDuration))
            .Insert(offset + fadeoffset + eachOffset * 0.5f * 2, buttonHolder3.DOFade(on ? 1 : 0, buttonsDuration))
            .Insert(offset + fadeoffset + eachOffset * 0.5f * 3, buttonHolder4.DOFade(on ? 1 : 0, buttonsDuration))
            .Insert(offset + eachOffset * 0,
                buttonHolder1.transform.DOLocalMoveY(on ? 0 : (farther ? -530 : -40), buttonsDuration)
                    .SetEase(Ease.OutBack))
            .Insert(offset + eachOffset * 1,
                buttonHolder2.transform.DOLocalMoveY(on ? 0 : (farther ? -530 : -40), buttonsDuration)
                    .SetEase(Ease.OutBack))
            .Insert(offset + eachOffset * 2,
                buttonHolder3.transform.DOLocalMoveY(on ? 0 : (farther ? -530 : -40), buttonsDuration)
                    .SetEase(Ease.OutBack))
            .Insert(offset + eachOffset * 3,
                buttonHolder4.transform.DOLocalMoveY(on ? 0 : (farther ? -530 : -40), buttonsDuration)
                    .SetEase(Ease.OutBack));
    }
}