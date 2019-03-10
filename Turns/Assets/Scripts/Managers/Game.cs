using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gamelogic.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

public class Game : Singleton<Game>
{
    public CameraFollow cameraFollow;
    public bool PlayerRunning { get; private set; }
    // time player takes to pass a tile
    public float initialTilePassTime = 0.4f;    
    public int InitialTiles = 6;
    public int PlayerPassTilesBuffer = 1;
    public int PathChangeProbability = 7;
    public int MaxStage = 10;
    public int TilesInStage = 4;
    public int MovesMade { get; private set; }
    
    float _startTime;
    int stage = 0;

    public float TilePassTime => Mathf.Lerp(0.1f, initialTilePassTime, (MaxStage - stage) / (float)MaxStage);

    public void Reset()
    {
        PlayerRunning = false;
        MovesMade = 0;
        stage = 0;
    }

    public void Play()
    {
        Reset();        
        Player.Instance.Reset();
        
        StartCoroutine(BeginAfterCountdown());
    }

    IEnumerator BeginAfterCountdown()
    {
        FloorManager.Instance.ResetAndPlay();
        Player.Instance.Play();
        yield return new WaitForSeconds(0.6f);
        PlayerRunning = true;
        _startTime = Time.fixedTime;
    }

    public void UserTap()
    {
        if (Game.Instance.PlayerRunning)
        {
            Player.Instance.ChangeDirection();
            MovesMade++;
            GameManager.Instance.UpdateUI();
            stage = MovesMade / TilesInStage;
        }
    }
    
    public void PlayerDie()
    {
        PlayerRunning = false;
        Player.Instance.SlowDownAndDie();
        GameManager.Instance.GameOver();
    }
}
