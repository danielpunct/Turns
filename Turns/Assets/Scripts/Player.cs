using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using TMPro;
using UnityEngine;

public class Player : Singleton<Player>
{
    bool _isCurrentlyDieing;
    Vector3Int _direction = VectorInt.forward;
    Rigidbody _rb;
    Transform _tr;
    Vector3Int? _currentTilePosition = Vector3Int.zero;

    float _dieingInertia = 1;
    float _startDieTime;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _tr = transform;
        _dieingInertia = 1;
    }

    private void FixedUpdate()
    {
        if (!Game.Instance.GameStarted && !_isCurrentlyDieing)
        {
            return;
        }

        if (_isCurrentlyDieing)
        {
            if (_dieingInertia > 0.001f)
            {
                _dieingInertia *= 0.9f;

            }
        }

        if (Time.unscaledTime - _startDieTime > 4)
        {
            _isCurrentlyDieing = false;
        }


        _rb.MovePosition(_tr.localPosition +
                         new Vector3(_direction.x * Speed, _direction.y * Speed, _direction.z * Speed));

        CheckTilePosition();
    }

    public void Reset()
    {
        _isCurrentlyDieing = false;
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
                }
                else
                {
                    _direction = direction;
                }
            }
        }
    }

    public void SlowDownAndDie()
    {
        _startDieTime = Time.unscaledTime;
        _isCurrentlyDieing = true;
    }

    float Speed
    {
        get { return (1 / Game.Instance.InitialTilePassTime * Time.fixedDeltaTime) * _dieingInertia; }
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
        
        var tile = FloorManager.Instance.PeekTile(_currentTilePosition.Value);

        if (tile == null && Game.Instance.GameStarted)
        {
            Game.Instance.PlayerDie();
        }   
         
    }
}
