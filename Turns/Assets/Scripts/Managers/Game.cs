using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gamelogic.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Game : Singleton<Game>
{
    public bool IsStarted { get; private set; }

    // time player takes to pass a tile
    public float initialTilePassTime = 0.4f;
    public float fastestTilePassTime = 0.1f;
    public int InitialTiles = 6;
    public int PlayerPassTilesBuffer = 1; // initial player wait
    public int StateChangeProbability = 7;
    [Range(0,100)]
    public int HoleChangePondere = 50;

    public int MaxStage = 10;
    public int TilesInStage = 4;
    public int DirChageMinDistance = 1;
    public int HoleLength = 2;
    public int HolesMinDistance = 2;
    public int MovesMade { get; private set; }

    float _startTime;
    int stage = 0;

    public float TilePassTime =>
        Mathf.Lerp(fastestTilePassTime, initialTilePassTime, (MaxStage - stage) / (float) MaxStage);

    public void Reset()
    {
        MovesMade = 0;
        stage = 0;
        FloorManager.Instance.Reset();
        Player.Instance.Reset();
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
        Player.Instance.Play();
        CameraFollow.Instance.SetForGame();
        yield return new WaitForSeconds(0.6f);
        _startTime = Time.fixedTime;
    }

    public void UserTap()
    {
        if (Player.Instance.IsRunning && !Player.Instance.IsJumping)
        {
            OperationsManager.Instance.DoNextAction();
            MovesMade++;
            GameManager.Instance.UpdateUI();
            stage = MovesMade / TilesInStage;
        }
    }

    public void PlayerDie()
    {
        Player.Instance.SlowDownAndDie();
        GameManager.Instance.GameOver();
        IsStarted = false;
    }
}