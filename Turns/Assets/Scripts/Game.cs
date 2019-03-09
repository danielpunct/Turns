using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gamelogic.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

public class Game : Singleton<Game>
{
    public bool GameStarted { get; private set; }
    // time player takes to pass a tile
    public float InitialTilePassTime = 0.4f;    
    public int InitialTiles = 6;
    public int PlayerPassTilesBuffer = 1;
    public int PathChangeProbability = 7;

    public float StartTime;
    
    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    void Reset()
    {
        Player.Instance.Reset();
        GameStarted = false;
        StartCoroutine(BeginAfterCountdown());
    }

    IEnumerator BeginAfterCountdown()
    {
        yield return new WaitForSeconds(1);
        GameStarted = true;
        StartTime = Time.fixedTime;
    }

    public void UserTap()
    {
        Player.Instance.ChangeDirection();
    }
    
    public void PlayerDie()
    {
        Debug.Log("dieded");
        GameStarted = false;
        Player.Instance.SlowDownAndDie();
    }
}
