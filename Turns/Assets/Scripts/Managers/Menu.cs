using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gamelogic.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

public class Menu : Singleton<Menu>
{
    [Header("Menu")] public GameObject menuUIHolder;
    public CanvasGroup lateHolder;
    public CanvasGroup buttonHolder1;
    public CanvasGroup buttonHolder2;
    public CanvasGroup buttonHolder3;
    public CanvasGroup buttonHolder4;

    [Header("Game")] public GameObject gameUIHolder;

    Sequence _menuSeq;

    void Awake()
    {
        ResetMenuUI();
    }

    void ResetMenuUI()
    {
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

        var buttonsDuration = 0.35f;
        var buttonsTime = 3;
        var buttonOffset = 0.07f;

        _menuSeq = DOTween.Sequence()
            .Insert(1, lateHolder.DOFade(1, 1f))
            .Insert(buttonsTime + buttonOffset*0.5f * 0, buttonHolder1.DOFade(1, buttonsDuration))
            .Insert(buttonsTime + buttonOffset*0.5f * 1, buttonHolder2.DOFade(1, buttonsDuration))
            .Insert(buttonsTime + buttonOffset*0.5f * 2, buttonHolder3.DOFade(1, buttonsDuration))
            .Insert(buttonsTime + buttonOffset*0.5f * 3, buttonHolder4.DOFade(1, buttonsDuration))
            .Insert(buttonsTime + buttonOffset * 0, buttonHolder1.transform.DOLocalMoveY(0, buttonsDuration).SetEase(Ease.OutBack))
            .Insert(buttonsTime + buttonOffset * 1, buttonHolder2.transform.DOLocalMoveY(0, buttonsDuration).SetEase(Ease.OutBack))
            .Insert(buttonsTime + buttonOffset * 2, buttonHolder3.transform.DOLocalMoveY(0, buttonsDuration).SetEase(Ease.OutBack))
            .Insert(buttonsTime + buttonOffset * 3, buttonHolder4.transform.DOLocalMoveY(0, buttonsDuration).SetEase(Ease.OutBack))
            .InsertCallback(buttonsTime, ResetElements);
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
}