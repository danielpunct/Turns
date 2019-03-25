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
    public int HoleChangePondere = 50;
    [Range(0,100)]
    public int StairePondere = 50;

    public int MaxStage = 10;
    public int MovesInStage = 4;
    public int DirChageMinDistance = 1;
    public int HoleLength = 2;
    public int HolesMinDistance = 2;
    public int StairsLength = 2;
    public int StairsMinDistance = 2;
    public int MovesMade { get; private set; }
    [ReadOnly]
    public Vector3 DefaultGravity;

    float _startTime;
    public int Stage { get; private set; }
    public int lastInteractionMove = 0;

    void Awake()
    {
        DefaultGravity = Physics.gravity;
    }

    public float TilePassTime =>
        Mathf.Lerp(fastestTilePassTime, initialTilePassTime, (MaxStage - Stage) / (float) MaxStage);

    public void Reset()
    {
        MovesMade = 0;
        lastInteractionMove = -10;
        Stage = 0;
        FloorManager.Instance.Reset();
        Runner.Instance.Reset();
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
        if (FloorManager.Instance.TilesPassed - lastInteractionMove < 2)
        {
            return;
        }

        lastInteractionMove = FloorManager.Instance.TilesPassed;
        
        if (Runner.Instance.IsRunning && !Runner.Instance.IsJumping )
        {
            var tile = FloorManager.Instance.PeekTile(Runner.Instance.CurrentTilePosition.Value);
            if (tile == null || tile.IsHole)
            {
                Debug.Log(tile == null ? "nul ": " hole");
                //return;
            }

            OperationsManager.Instance.DoNextAction();
            MovesMade++;
            Stage = MovesMade / MovesInStage;
            Menu.Instance.UpdateUI();
            Physics.gravity = DefaultGravity * 1 / TilePassTime;
        }
    }

    public void PlayerDie()
    {
        GameManager.Instance.Player.SaveRun(MovesMade, Time.fixedTime - _startTime, FloorManager.Instance.TilesPassed);
        Runner.Instance.SlowDownAndDie();
        GameManager.Instance.GameOver();
        IsStarted = false;
    }
}