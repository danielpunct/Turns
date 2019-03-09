using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

public class Game : Singleton<Game>
{
    public bool GameStarted { get; set; }
    // time player takes to pass a tile
    public float InitialTilePassTime = 0.4f;    
    public int InitialTiles = 6;
    public int PlayerPassTilesBuffer = 1;

    public float StartTime;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BeginAfterCountdown());
    }

    IEnumerator BeginAfterCountdown()
    {
        yield return new WaitForSeconds(1);
        GameStarted = true;
        StartTime = Time.fixedTime;
    }
}
