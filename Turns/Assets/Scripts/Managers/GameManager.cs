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


    public void StartAnotherGame(bool continueLevel)
    {
        Game.Instance.ResetAndPlay(continueLevel);
        Menu.Instance.ShowGameMenu();
        Menu.Instance.UpdateUI();
    }

    public void LevelFailed()
    {
        Player.SaveRun(
            Game.Instance.MovesMade,
            Game.Instance.GameDuration,
            FloorManager.Instance.TilesPassed,
            Game.Instance.PerfectPoints,
            Game.Instance.CurrentStage - 1,
            true);

        Menu.Instance.ShowMenu(false);
        Game.Instance.IsStarted = false;
    }

    public void LevelPassed()
    {
        Game.Instance.ResetWorld(true);
        StartAnotherGame(true);
    }
    
}
