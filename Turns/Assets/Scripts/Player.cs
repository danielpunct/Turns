using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using TMPro;
using UnityEngine;

public class Player : Singleton<Player>
{
    private Vector3Int _direction = VectorInt.forward;
    private Rigidbody _rb;
    private Transform _tr;
    private Vector3Int? _currentTilePosition = Vector3Int.zero;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _tr = transform;
    }

    private void FixedUpdate()
    {
        if (!Game.Instance.GameStarted)
        {
            return;
        }

        _rb.MovePosition(_tr.localPosition +
                         new Vector3(_direction.x * Speed, _direction.y * Speed, _direction.z * Speed));
        
        CheckTilePosition();
    }

    public void ChangeDirection()
    {
        if (_currentTilePosition != null)
        {
            var tile = FloorManager.Instance.PeekTile(_currentTilePosition.Value);

            if (tile != null && tile.NextPositionKey != null)
            {
                var direction = tile.NextPositionKey.Value - _currentTilePosition.Value;
                if (direction == _direction)
                {
                    _direction =
                        FloorManager.Instance.GetNextPathChangeDirection(_currentTilePosition.Value, _direction);
                    // end game?
                }
                else
                {
                    _direction = direction;
                }
            }
        }
    }

    float Speed
    {
        get { return 1 / Game.Instance.InitialTilePassTime * Time.fixedDeltaTime; }
    }

    void CheckTilePosition()
    {
        var pos = _tr.localPosition;

        var tilePosition = new Vector3Int(Mathf.RoundToInt(pos.x), 0, Mathf.RoundToInt(pos.z));

        if (_currentTilePosition != null && tilePosition != _currentTilePosition)
        {
            FloorManager.Instance.OnPlayerPassedTile();
        }

        _currentTilePosition = tilePosition;
    }
}
