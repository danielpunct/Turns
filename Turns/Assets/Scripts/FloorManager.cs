using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions;
using UnityEngine;

public class FloorManager : Singleton<FloorManager>
{
    public Transform tilesHolder;
    
    Queue<FloorTile> tiles;
    Vector3 nextTilePosition = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitialSetup());
    }

    IEnumerator InitialSetup()
    {
        tiles = new Queue<FloorTile>();

        for (int i = 0; i < Game.Instance.InitialTiles; i++)
        {
            AppearNewTile();
            SetNextPosition(Vector3.forward);
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void OnPlayerPassedTile()
    {
        if (Game.Instance.PlayerPassTilesBuffer-- > 0)
        {
            return;
        }
        if (tiles == null || tiles.Count < Game.Instance.InitialTiles)
        {
            return;
        }
        var lastTile = tiles.Dequeue();
        
        lastTile.Dissapear();
        
        AppearNewTile();
        SetNextPosition(Vector3.forward);
    }
    

    public void SetNextPosition(Vector3 direction)
    {
        nextTilePosition += direction;
    }

    public FloorTile AppearNewTile()
    {
        var tile = PoolManager.Instance.TilesPool.Spawn(nextTilePosition, Quaternion.identity, tilesHolder)
            .GetComponent<FloorTile>();
        tile.Appear();
        
        tiles.Enqueue(tile);

        
        return tile;
    }
}