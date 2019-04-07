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
    public bool IsStarted { get; private set; }

    // time player takes to pass a tile
    public float initialTilePassTime = 0.4f;
    public float fastestTilePassTime = 0.1f;
    public int InitialTiles = 6;
    public int PlayerPassTilesBuffer = 1; // initial player wait
    public int StateChangeChances = 7;
    [Range(0,100)]
    public int HolePondere = 50;
    [Range(0,100)]
    public int StairePondere = 50;
    [Range(0,100)]
    public int DirChangePondere = 50;

    public int MaxStage = 10;
    public int TilesInStage = 4;
    public int DirChageMinDistance = 1;
    public int HoleLength = 2;
    public int HolesMinDistance = 2;
    public int StairsLength = 2;
    public int StairsMinDistance = 2;
    public float PerfectChangeThreshold = 0.1f;
    public int MovesMade { get; private set; }
    public int PerfectPoints { get; private set; }
    [ReadOnly]
    public Vector3 DefaultGravity;

    public int Stage { get; private set; }
    public int LastInteractionMove = 0;

    float _startTime;
    int perfectChangeMove = -1;
    int perfectChangeBuffer = 1;
    
    void Awake()
    {
        DefaultGravity = Physics.gravity;
    }

    public float TilePassTime =>
        Mathf.Lerp(fastestTilePassTime, initialTilePassTime, (MaxStage - Stage) / (float) MaxStage);

    public void Reset()
    {
        MovesMade = 0;
        PerfectPoints = 0;
        LastInteractionMove = -10;
        perfectChangeMove = -1;
        perfectChangeBuffer = 1;
        Stage = 0;
        
    }

    public void Play()
    {
        Reset();

        StartCoroutine(BeginAfterCountdown());
        IsStarted = true;
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
        if (FloorManager.Instance.TilesPassed - LastInteractionMove < 2)
        {
            return;
        }

        
        if (Runner.Instance.State == Runner.RunnerState.Running )
        {
            OperationsManager.Instance.DoNextAction();
            LastInteractionMove = FloorManager.Instance.TilesPassed;

            MovesMade++;
            Stage = FloorManager.Instance.TilesPassed / TilesInStage;
            Menu.Instance.UpdateUI();
            Physics.gravity = DefaultGravity * 1 / TilePassTime;
        }
    }


    public void OnPlayerPerfectChange()
    {
        if (perfectChangeMove == MovesMade - 1)
        {
            perfectChangeBuffer += 2;
        }
        else
        {
            perfectChangeBuffer = 1;
        }

        perfectChangeMove = MovesMade;

//        Menu.Instance.comboUI.Show(perfectChangeBuffer + "x");
        Menu.Instance.comboUI.Show("+"+perfectChangeBuffer);
        PerfectPoints += perfectChangeBuffer;
    }

    public void RunOver(Vector3Int? awayDirection = null)
    {
        GameManager.Instance.Player.SaveRun(
            MovesMade, 
            Time.fixedTime - _startTime, 
            FloorManager.Instance.TilesPassed,
            PerfectPoints);

        GameManager.Instance.GameOver();
        IsStarted = false;
    }
}