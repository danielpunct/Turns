using System.Collections;
using DG.Tweening;
using Gamelogic.Extensions;
using UnityEngine;

public class Menu : Singleton<Menu>
{
    public ColorSuite cameraColorSuite;
    
    public MenuScreens menuUI;
    public GameScreens gameUI;
    
    Sequence _2ndMenuSeq;

    public void OnStartApp()
    {
        ShowMenuScreen(MenuScreens.MenuState.init);
    }
    
    public void OnTilePass()
    {
        if (Game.Instance.IsStarted)
        {
            gameUI.UpdateState();
        }
    }

    public void OnLevelFailed()
    {
        if (Game.Instance.SkipContinue)
        {
            ShowMenuScreen(MenuScreens.MenuState.gameOver);
        }
        else
        {
            ShowMenuScreen(MenuScreens.MenuState.continueRun);
        }
    }

    public void OnLevelStartWarp()
    {
        ShowGameScreen(GameScreens.GameState.levelTransition);
    }
    
    public void OnLevelWarped()
    {
        ShowGameScreen(GameScreens.GameState.play);
    }

    public void OnDefaultPlayClick()
    {
        GameManager.Instance.StartAnotherGame(GameManager.ContinueMode.startOver);
        ShowGameScreen(GameScreens.GameState.play);
    }
    
    public void OnNoThanksClick()
    {
        ShowMenuScreen(MenuScreens.MenuState.gameOver);
    }


    public void OnContinueClick()
    {
        GameManager.Instance.StartAnotherGame(GameManager.ContinueMode.levelRepeat);
        ShowGameScreen(GameScreens.GameState.play);
    }

    public void OnGameOverConfirmClick()
    {
        GameManager.Instance.StartAnotherGame(GameManager.ContinueMode.startOver);
        ShowGameScreen(GameScreens.GameState.play);
    }

    
    void ShowMenuScreen(MenuScreens.MenuState state)
    {
        gameUI.Hide();
        menuUI.Show(state);

        StopCoroutine(SaturateImage(1));
        StopCoroutine(SaturateImage(0));
        
        switch (state)
        {
            case MenuScreens.MenuState.init:
                StartCoroutine(SaturateImage(1));
                break;
            case MenuScreens.MenuState.home:
                StartCoroutine(SaturateImage(1));
                break;
            case MenuScreens.MenuState.continueRun:
                StartCoroutine(SaturateImage(0));
                break;
            case MenuScreens.MenuState.gameOver:
                StartCoroutine(SaturateImage(1));
                break;
        }
    }

    void ShowGameScreen(GameScreens.GameState state)
    {
        gameUI.Show(state);
        menuUI.Hide();
        
        StopCoroutine(SaturateImage(1));
        StopCoroutine(SaturateImage(0));
        
        StartCoroutine(SaturateImage(1));
    }
    
    IEnumerator SaturateImage(float endValue)
    {
        yield return null;
        var init = cameraColorSuite.saturation;
        for (int i = 1; i < 20; i++)
        {
            cameraColorSuite.saturation = Mathf.Lerp(init, endValue, i / 20f);
            yield return null;
        }
    }
}