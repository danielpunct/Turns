using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;

public class FloorManager : Singleton<FloorManager>
{
    public Transform tilesHolder;

    Dictionary<Vector3Int, FloorTile> _tilesDict;
    LinkedList<FloorTile> _tiles;
    Vector3Int _nextTilePosition = Vector3Int.zero;
    Vector3Int _currentDirection = VectorInt.forward;

    public void ResetAndPlay()
    {
        _nextTilePosition = Vector3Int.zero;
        _currentDirection = VectorInt.forward;
        StartCoroutine(InitialSetup());
    }

    IEnumerator InitialSetup()
    {
        PoolManager.Instance.TilesPool.DespawnAll();
        _tiles = new LinkedList<FloorTile>();
        _tilesDict = new Dictionary<Vector3Int, FloorTile>();

        for (int i = 0; i < Game.Instance.InitialTiles; i++)
        {
            AppearNewTile();
            SetNextPosition(true);

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void OnPlayerPassedTile()
    {
        if (Game.Instance.PlayerPassTilesBuffer-- > 0)
        {
            return;
        }

        if (_tiles == null || _tiles.Count < Game.Instance.InitialTiles)
        {
            return;
        }

        var lastTile = DeregisterFirstTile();

        lastTile.Dissapear();

        AppearNewTile();
        SetNextPosition(false);
    }


    public void SetNextPosition(bool init)
    {
        if (!init)
        {
            if (Random.Range(0, Game.Instance.PathChangeProbability) == 0) // if change direction
            {
                _currentDirection = _currentDirection.GetChangedRandomDirection();
            }
        }

        _nextTilePosition += _currentDirection;
    }



    public FloorTile AppearNewTile()
    {
        var tile = PoolManager.Instance.TilesPool.Spawn(_nextTilePosition, Quaternion.identity, tilesHolder)
            .GetComponent<FloorTile>();

        tile.Appear();

        RegisterLastTile(tile, _nextTilePosition);

        return tile;
    }

    void RegisterLastTile(FloorTile tile, Vector3Int atPosition)
    {
        tile.PositionKey = atPosition;
        if (_tiles.Count > 0)
        {
            _tiles.Last.Value.NextPositionKey = atPosition;
        }

        _tiles.AddLast(tile);
        _tilesDict.Add(atPosition, tile);
    }

    FloorTile DeregisterFirstTile()
    {
        var lastTile = _tiles.First.Value;
        _tiles.RemoveFirst();
        _tilesDict.Remove(lastTile.PositionKey);
        return lastTile;
    }

    public FloorTile PeekTile(Vector3Int atPosition)
    {
        return !_tilesDict.ContainsKey(atPosition) ? null : _tilesDict[atPosition];
    }

    public Vector3Int GetNextPathChangeDirection(Vector3Int fromPositionKey, Vector3Int currentDirection)
    {
        var tile = _tiles.Find(_tilesDict[fromPositionKey]);

        while (tile != null)
        {
            if (tile.Value.NextPositionKey == null)
            {
                break;
            }
            var direction = tile.Value.NextPositionKey - tile.Value.PositionKey;         

            if (direction.Value != currentDirection)
            {
                return direction.Value;
            }
            
            if(Game.Instance.GameStarted)
            {
                // if first current tile is not the corner, player dies
                Game.Instance.PlayerDie();
            }

            tile = tile.Next;
        }

        Game.Instance.PlayerDie();
        return currentDirection.GetChangedRandomDirection();
    }
}