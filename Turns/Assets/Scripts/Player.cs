using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gamelogic.Extensions;
using TMPro;
using UnityEngine;

public class Player : Singleton<Player>
{
    public bool IsFalling { get; private set; }
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
        if (!Game.Instance.PlayerRunning && !IsFalling)
        {
            return;
        }

        if (IsFalling)
        {
            if (_dieingInertia > 0.001f)
            {
                _dieingInertia *= 0.9f;

            }
        }

        if (IsFalling && Time.unscaledTime - _startDieTime > 4)
        {
           Reset();
        }

        _rb.MovePosition(_tr.localPosition +
                         new Vector3(_direction.x * Speed, _direction.y * Speed, _direction.z * Speed));

        if (!IsFalling)
        {
            CheckTilePosition();
        }
    }

    public void Reset()
    {
        _rb.isKinematic = true;
        _tr.localScale = Vector3.zero;
        _tr.localPosition = Vector3.up * 2f;
        IsFalling = false;
        _currentTilePosition = Vector3Int.zero;
        _rb.velocity = Vector3.zero;
        _rb.rotation = Quaternion.identity;
        _tr.rotation = Quaternion.identity;
        _dieingInertia = 1;
        _direction = VectorInt.forward;
    }

    public void Play()
    {
        _tr.DOScale(1, 0.6f).SetEase(Ease.OutBack);
        _rb.isKinematic = false;
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
        IsFalling = true;
    }

    float Speed
    {
        get { return (1 / Game.Instance.TilePassTime * Time.fixedDeltaTime) * _dieingInertia; }
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

        if (tile == null && Game.Instance.PlayerRunning)
        {
            Game.Instance.PlayerDie();
        }   
         
    }
}
