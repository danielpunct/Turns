using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gamelogic.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Game : Singleton<Game>
{
    public TMP_Text _debug;
    public bool IsStarted { get; set; }

    // time player takes to pass a tile
    public float initialTilePassTime = 0.4f;
    public float fastestTilePassTime = 0.1f;
    public int InitialTiles = 6;
    public int PlayerPassTilesBuffer = 1; // initial player wait
    public int StateChangeChances = 7;
    [Range(0, 100)] public int HolePondere = 50;
    [Range(0, 100)] public int StairePondere = 50;
    [Range(0, 100)] public int DirChangePondere = 50;

    public int MaxStage = 10;
    public int TilesInStage = 4;
    public int DirChageMinDistance = 1;
    public int HoleLength = 2;
    public int HolesMinDistance = 2;
    public int StairsLength = 2;
    public int StairsMinDistance = 2;
    public float PerfectChangeThreshold = 0.1f;
    [ReadOnly] public Vector3 DefaultGravity;

   
    public int MovesMade { get; private set; }
    public int PerfectPoints { get; private set; }
    public bool StageFinished { get; private set; }
    public int CurrentStage { get; private set; }
    
    public int Points => PerfectPoints + MovesMade;


    int _previousMovesMade;
    int _previousPerfectPoints;
    int _lastInteractionMove = 0;
    float _startTime;
    int _perfectChangeMove = -1;
    int _perfectChangeBuffer = 1;

    void Awake()
    {
        DefaultGravity = Physics.gravity;
    }

    public float TilePassTime =>
        Mathf.Lerp(fastestTilePassTime, initialTilePassTime, (MaxStage - CurrentStage) / (float) MaxStage);
    
    public bool StageFinishable =>
        FloorManager.Instance.TilesPassed / TilesInStage > 0;

    public float GameDuration => Time.fixedTime - _startTime;

    public void ResetAndPlay(GameManager.ContinueMode mode)
    {
        switch (mode)
        {
            case GameManager.ContinueMode.levelPassed:
                CurrentStage++;
                _previousMovesMade = MovesMade;
                _previousPerfectPoints = PerfectPoints;
                break;
            case GameManager.ContinueMode.levelRepeat:
                MovesMade = _previousMovesMade;
                PerfectPoints = _previousPerfectPoints;
                break;
            case GameManager.ContinueMode.startOver: 
                MovesMade = 0;
                PerfectPoints = 0;
                _previousMovesMade = 0;
                _previousPerfectPoints = 0;
                CurrentStage = GameManager.Instance.Player.LevelSelected + 1;
                break;
        }

        _lastInteractionMove = -10;
        _perfectChangeMove = -1;
        _perfectChangeBuffer = 1;
        StageFinished = false;

        StartCoroutine(BeginAfterCountdown(mode == GameManager.ContinueMode.levelPassed));
        IsStarted = true;
    }
    
    public void ResetWorld(GameManager.ContinueMode mode)
    {
        Runner.Instance._Reset(); // need to be done before camera
        switch (mode)
        {
            case GameManager.ContinueMode.levelPassed:
                break;
            case GameManager.ContinueMode.levelRepeat:
                CameraFollow.Instance.SetForGame(false);
                break;
            case GameManager.ContinueMode.startOver:
                CameraFollow.Instance.SetForMenu();
                break;
        }
        {
           
        }

        FloorManager.Instance._Reset();
        MomentsRecorderHelper.Instance.StopReplay();
    }

    IEnumerator BeginAfterCountdown(bool continueLevel)
    {
        FloorManager.Instance.Play();
        Runner.Instance.Play();
        CameraFollow.Instance.SetForGame(continueLevel);
        yield return new WaitForSeconds(0.6f);
        _startTime = Time.fixedTime;
    }

    public void UserTap()
    {
        if (FloorManager.Instance.TilesPassed - _lastInteractionMove < 2 || Runner.Instance.IsWinWalking)
        {
            return;
        }


        if (Runner.Instance.State == Runner.RunnerState.Running)
        {
            OperationsManager.Instance.DoNextAction();
            _lastInteractionMove = FloorManager.Instance.TilesPassed;

            MovesMade++;
            Physics.gravity = DefaultGravity * 1 / TilePassTime;
        }
    }

    public void OnRunnerPassTile()
    {
        Menu.Instance.OnTilePass();
        if (Runner.Instance.State != Runner.RunnerState.Cinematic && Runner.Instance.State != Runner.RunnerState.EndCinematic)
        {
            Physics.gravity = DefaultGravity * 1 / TilePassTime;
        }

    }

    public void OnRunnerPerfectChange()
    {
        if (_perfectChangeMove == MovesMade - 1)
        {
            _perfectChangeBuffer += _perfectChangeBuffer == 1 ? 1 : 2;
        }
        else
        {
            _perfectChangeBuffer = 1;
        }

        _perfectChangeMove = MovesMade;

        Menu.Instance.gameUI.DisplayComboPoints(_perfectChangeBuffer);
        PerfectPoints += _perfectChangeBuffer;
    }

    public void OnRunnerFallOver(Vector3Int? awayDirection = null)
    {
        GameManager.Instance.LevelFailed();
    }


    public void OnRunnerJumpToWarp()
    {
        Menu.Instance.OnLevelStartWarp();
        Runner.Instance.Jump_EndLevel();
    }
    
    
    public void OnRunnerWarped()
    {
        StageFinished = true;
        GameManager.Instance.LevelPassed();
    }

}