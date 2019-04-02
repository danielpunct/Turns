using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;

public class FloorManager : Singleton<FloorManager>
{
    public Transform tilesHolder;
    public int TilesPassed { get; private set; }
    
    Dictionary<Vector3Int, FloorTile> _tilesDict;
    LinkedList<FloorTile> _tiles;
    Vector3Int _currentDirection = VectorInt.forward;
    int passTileBufffer;
    
    
    // --- NEXT TILE STATE --- 
    Vector3Int _nextTilePosition = Vector3Int.zero;
    int _nextHoleBuffer = 0; // units of hole left
    int _nextDirChangeBuffer = 0;
    int _nextStairsBuffer = 0;
    bool _nextStairDirectionUp;

    public void Reset()
    {
        passTileBufffer = Game.Instance.PlayerPassTilesBuffer;
        _nextTilePosition = Vector3Int.zero;
        _currentDirection = VectorInt.forward;
        PoolManager.Instance.TilesPool.DespawnAll();
        _nextHoleBuffer = 0;
        _nextDirChangeBuffer = 0;
        _nextStairsBuffer = 0;
        TilesPassed = 0;
    }

    public void Play()
    {
        _tiles = new LinkedList<FloorTile>();
        _tilesDict = new Dictionary<Vector3Int, FloorTile>();
        StartCoroutine(InitialSetup());
    }

    IEnumerator InitialSetup()
    {
        for (int i = 0; i < Game.Instance.InitialTiles; i++)
        {
            AppearNewTile();
            PrepareNextTileState(true);

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void OnPlayerPassedTile()
    {
        if (passTileBufffer-- > 0)
        {
            return;
        }

        if (_tiles == null || _tiles.Count < Game.Instance.InitialTiles)
        {
            return;
        }

        TilesPassed++;

        var lastTile = DeregisterFirstTile();

        lastTile.Dissapear();

        AppearNewTile();
        PrepareNextTileState(false);
        
        Menu.Instance.UpdateUI();
    }

   

    public FloorTile AppearNewTile()
    {
        var tile = PoolManager.Instance.TilesPool.Spawn(_nextTilePosition, Quaternion.identity, tilesHolder)
            .GetComponent<FloorTile>();

        tile.Appear(
            _nextHoleBuffer > 0,
            _nextStairsBuffer > 0 ? (_nextStairDirectionUp ? 1 : -1) : 0,
            _nextTilePosition);

        RegisterLastTile(tile, _nextTilePosition);

        return tile;
    }

    void RegisterLastTile(FloorTile tile, Vector3Int atPosition)
    {
        
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

    public Vector3Int GetChangedRandomDirection(Vector3Int dir, Vector3Int? fromPosition = null)
    {
        var safeint = 30;
        while (true)
        {
            var newDirection = VectorInt.Directions[Random.Range(0, 4)];

            if (newDirection.x == dir.x || newDirection.z == dir.z)
            {
                continue;
            }

            if (fromPosition != null && !IsSafeDirection(fromPosition.Value, newDirection))
            {
                if (safeint-- > 0)
                {
                    continue;
                }

                Debug.LogError("safe direction !");
            }

            return newDirection;
        }
    }

    bool IsSafeDirection(Vector3Int fromPosition, Vector3Int inDirection)
    {
        for (var i = 1; i <= Game.Instance.InitialTiles * 2 / 3; i++)
        {
            if (_tilesDict.ContainsKey(fromPosition + inDirection * i))
            {
                return false;
            }
        }

        return true;
    }


    public OperationsManager.PlayerAction GetNextPendingOperation(Vector3Int fromPositionKey, Vector3Int currentDirection)
    {
        var tile = _tiles.Find(_tilesDict[fromPositionKey]);

        while (tile != null)
        {
            // if it is the last tile generated
            if (tile.Value.NextPositionKey == null)
            {
                break;
            }

            if (tile.Next!=null && tile.Next.Value.IsHole)
            {
                if (Game.Instance.IsStarted)
                {
                    return OperationsManager.PlayerAction.Jump;
                }
                else // if user pressed jump too fast and died in the previous run
                {
                    return OperationsManager.PlayerAction.None;
                }
            }

            var direction = tile.Value.NextPositionKey.Value.HorizontalDif(tile.Value.PositionKey);

            if (direction != currentDirection)
            {
                return direction.ToAction();
            }

            tile = tile.Next;
        }

        return OperationsManager.PlayerAction.None;
    }

    bool changeQueued;
    
    public void PrepareNextTileState(bool init)
    {
        if (!init)
        {
            var changeDirAllowed = 
                _nextDirChangeBuffer <= -Game.Instance.DirChageMinDistance &&
                _nextHoleBuffer <= -1 &&
                _nextStairsBuffer <= 0;
            var holeAllowed = 
                _nextHoleBuffer <= -Game.Instance.HolesMinDistance &&
                _nextDirChangeBuffer <= -1 &&
                _nextStairsBuffer <= -1;
            var stairsAllowed = 
                _nextStairsBuffer <= -Game.Instance.StairsMinDistance &&
                _nextDirChangeBuffer <= -1 &&
                _nextHoleBuffer <= -1;
           
            _nextDirChangeBuffer--;
            _nextHoleBuffer--;
            _nextStairsBuffer--;

            
            
            if (changeQueued 
                || Random.Range(0, Game.Instance.StateChangeChances) == 0
                || _nextDirChangeBuffer < -4) // if change state
            {
                changeQueued = true;
                var pondere = Random.Range(0,
                    Game.Instance.HolePondere + Game.Instance.StairePondere + Game.Instance.DirChangePondere);


                if (holeAllowed && pondere < Game.Instance.HolePondere) // if do hole
                {
                    _nextHoleBuffer = Game.Instance.HoleLength;
                    changeQueued = false;
                }
                else if (stairsAllowed &&  pondere < Game.Instance.HolePondere + Game.Instance.StairePondere) // if do stairs
                {
                    _nextStairDirectionUp = false; // Random.Range(0, 2) == 1;
                    _nextStairsBuffer = Game.Instance.StairsLength;
                    changeQueued = false;

                }
                else if (changeDirAllowed) // change dir
                {
                    _currentDirection = GetChangedRandomDirection(_currentDirection, _tiles.Last.Value.PositionKey);
                    _nextDirChangeBuffer = Game.Instance.DirChageMinDistance;
                    changeQueued = false;
                }
            }

            if (_nextStairsBuffer > 0)
            {
                _nextTilePosition.y += _nextStairDirectionUp ? 1 : -1;
            }
        }

        _nextTilePosition += _currentDirection;
    }
}