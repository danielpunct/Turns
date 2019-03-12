using DG.Tweening;
using Gamelogic.Extensions;
using UnityEngine;

public class Player : Singleton<Player>
{
    public PlayerController controller;
    public GameObject apearParticleSystem;
    public bool IsFalling { get; private set; }
    public bool IsRunning { get; private set; }
    public Vector3Int Direction { get; private set; }
    Rigidbody _rb;
    Transform _tr;
    Vector3Int? _currentTilePosition = Vector3Int.zero;

    float _dieingInertia = 1;
    float _startDieTime;
    Sequence _appearSequence;

    bool FallingInTheBeginning => IsFalling && Game.Instance.IsStarted;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _tr = transform;
        _dieingInertia = 1;
    }

    void FixedUpdate()
    {
        if (!IsRunning && !IsFalling)
        {
            return;
        }

        // is falling to it's death
        if (!Game.Instance.IsStarted && IsFalling)
        {
            if (_dieingInertia > 0.001f)
            {
                _dieingInertia *= 0.9f;
            }
        }

        // is falling to it's death for too long
        if (!Game.Instance.IsStarted && IsFalling && Time.unscaledTime - _startDieTime > 4)
        {
            Reset();
        }

        if (!FallingInTheBeginning)
        {
            _rb.MovePosition(_tr.localPosition +
                             new Vector3(Direction.x * Speed, Direction.y * Speed, Direction.z * Speed));
        }

        if (!IsFalling)
        {
            CheckTilePosition();
        }
    }

    public void Reset()
    {
        IsRunning = false;
        IsFalling = false;
        controller.Reset();
        _rb.isKinematic = true;
        _tr.localScale = Vector3.zero;
        _tr.localPosition = Vector3.up * 2f;
        _currentTilePosition = Vector3Int.zero;
        _rb.velocity = Vector3.zero;
        _rb.rotation = Quaternion.identity;
        _tr.rotation = Quaternion.identity;
        _dieingInertia = 1;
        Direction = VectorInt.forward;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        apearParticleSystem.SetActive(false);

    }

    public void Play()
    {
        if (_appearSequence != null)
        {
            _appearSequence.Kill();
        }

        _appearSequence = DOTween.Sequence()
            .Insert(0, _tr.DOScale(1, 0.6f).SetEase(Ease.OutBack))
            .InsertCallback(0.2f, () => { apearParticleSystem.SetActive(true); })
            .InsertCallback(1f, () =>
            {
                IsRunning = true;
                IsFalling = false;
            });

        IsFalling = true;
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
                if (direction == Direction)
                {
                    Direction =
                        FloorManager.Instance.GetNextPathChangeDirection(_currentTilePosition.Value, Direction);
                }
                else
                {
                    Direction = direction;
                }
            }
        }

        var eulerRotation = Quaternion.LookRotation(Direction, Vector3.up).eulerAngles;
        _rb.DORotate(eulerRotation, 0.4f).SetEase(Ease.OutBack);
    }

    public void SlowDownAndDie()
    {
        IsRunning = false;
        _startDieTime = Time.unscaledTime;

        _rb.constraints = RigidbodyConstraints.None;
        IsFalling = true;
    }

    public float Speed
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

        if (tile == null && IsRunning)
        {
            Game.Instance.PlayerDie();
        }
    }
}