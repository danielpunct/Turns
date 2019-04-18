using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public PlayerState Player;

    void Awake()
    {
        Player = new PlayerState();
        Player.Load();
        Application.targetFrameRate = 60;

    }

    void Start()
    {
        Menu.Instance.ShowMenu(true);
        Menu.Instance.UpdateUI();
    }


    public void StartAnotherGame()
    {
        Game.Instance.Play();
        Menu.Instance.ShowGameMenu();
        Menu.Instance.UpdateUI();
    }

    public void LevelOver(bool levelPassed)
    {
        if (!levelPassed)
        {
            Menu.Instance.ShowMenu(false);
            Game.Instance.IsStarted = false;
        }

        else
        {
            Game.Instance.ResetWorld();
            StartAnotherGame();
        }
    }
}
