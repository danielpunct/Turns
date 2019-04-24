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

    public void Reset()
    {
        MovesMade = 0;
        PerfectPoints = 0;
        _lastInteractionMove = -10;
        _perfectChangeMove = -1;
        _perfectChangeBuffer = 1;
        CurrentStage = GameManager.Instance.Player.LevelSelected + 1;
        StageFinished = false;
    }

    public void Play()
    {
        Reset();

        StartCoroutine(BeginAfterCountdown());
        IsStarted = true;
    }
    
    public void ResetWorld()
    {
        Runner.Instance.Reset(); // need to be done before camera
        CameraFollow.Instance.SetForMenu();
        FloorManager.Instance.Reset();
        MomentsRecorderHelper.Instance.StopReplay();
    }

    IEnumerator BeginAfterCountdown()
    {
        FloorManager.Instance.Play();
        Runner.Instance.Play();
        CameraFollow.Instance.SetForGame();
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
        Menu.Instance.UpdateUI();
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

//        Menu.Instance.comboUI.Show(perfectChangeBuffer + "x");
        Menu.Instance.comboUI.Show($"+{_perfectChangeBuffer}");
        PerfectPoints += _perfectChangeBuffer;
    }

    public void OnRunnerFallOver(Vector3Int? awayDirection = null)
    {
       OnRunDie();
    }

    public void OnRunnerJumpToWarp()
    {
        Menu.Instance.HideInGameMenu();
        Runner.Instance.Jump_EndLevel();
    }

    public void OnRunnerWarped()
    {
        CameraFollow.Instance.SetForGame();
        StageFinished = true;
        OnContinueNextLevel();
    }

    void OnRunDie()
    {
        GameManager.Instance.Player.SaveRun(
            MovesMade,
            Time.fixedTime - _startTime,
            FloorManager.Instance.TilesPassed,
            PerfectPoints,
            StageFinished ? CurrentStage : GameManager.Instance.Player.MaxLevelPassed,
            !StageFinished);

        GameManager.Instance.LevelOver(StageFinished);
    }

    void OnContinueNextLevel()
    {
        
    }
}