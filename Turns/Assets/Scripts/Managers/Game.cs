using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gamelogic.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

public class Game : Singleton<Game>
{
    public CameraFollow cameraFollow;
    public bool GameStarted { get; private set; }
    // time player takes to pass a tile
    public float InitialTilePassTime = 0.4f;    
    public int InitialTiles = 6;
    public int PlayerPassTilesBuffer = 1;
    public int PathChangeProbability = 7;
    public int MovesMade { get; private set; }

    public float StartTime;
    

    public void Reset()
    {
        GameStarted = false;
        MovesMade = 0;
    }

    public void Play()
    {
        MovesMade = 0;        
        Player.Instance.Reset();
        cameraFollow.Reset();
        
        StartCoroutine(BeginAfterCountdown());
    }

    IEnumerator BeginAfterCountdown()
    {
        FloorManager.Instance.ResetAndPlay();
        Player.Instance.Play();
        yield return new WaitForSeconds(0.6f);
        GameStarted = true;
        StartTime = Time.fixedTime;
    }

    public void UserTap()
    {
        if (Game.Instance.GameStarted)
        {
            Player.Instance.ChangeDirection();
            MovesMade++;
            GameManager.Instance.DisplayMoves();
        }
    }
    
    public void PlayerDie()
    {
        Debug.Log("dieded");
        GameStarted = false;
        Player.Instance.SlowDownAndDie();
        GameManager.Instance.GameOver();
    }
}
