using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public PlayerData Player;

    public enum ContinueMode
    {
        levelPassed, 
        levelRepeat,
        startOver
    }
    
    void Awake()
    {
        Player = new PlayerData();
        Player.Load();
        Application.targetFrameRate = 60;

    }

    void Start()
    {
        Menu.Instance.OnStartApp();
        Game.Instance.ResetWorld(ContinueMode.startOver);
    }


    public void StartAnotherGame(ContinueMode mode)
    {
        Game.Instance.ResetWorld(mode);
        Game.Instance.ResetAndPlay(mode);
    }

    public void LevelFailed()
    {
        Player.SaveRun(
            Game.Instance.MovesMade,
            Game.Instance.GameDuration,
            FloorManager.Instance.TilesPassed,
            Game.Instance.Points,
            Game.Instance.CurrentStage - 1,
            true);

        Menu.Instance.OnLevelFailed();
        Game.Instance.IsStarted = false;
    }

    public void LevelPassed()
    {
        Game.Instance.ResetWorld(ContinueMode.levelPassed);
        Game.Instance.ResetAndPlay(ContinueMode.levelPassed);
        
        Menu.Instance.OnLevelWarped();
    }
    
}
