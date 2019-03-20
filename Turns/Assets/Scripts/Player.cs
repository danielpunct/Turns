using DG.Tweening;
using Gamelogic.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : Singleton<Player>
{
    public GameObject apearParticleSystem;
    public bool IsFalling { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsJumping { get; private set; }
    public Vector3Int Direction { get; private set; }
    Rigidbody _rb;
    Transform _tr;
    public Vector3Int? CurrentTilePosition = Vector3Int.zero;

    float _dieingInertia = 1;
    float _startDieTime;
    Sequence _appearSequence;
    bool _pleaseJump;
    int jumpBuffer;

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
                _dieingInertia *= 0.94f;
            }
        }

        // is falling to it's death for too long
        if (!Game.Instance.IsStarted && IsFalling && Time.unscaledTime - _startDieTime > 6)
        {
            Reset();
        }

        if (!FallingInTheBeginning)
        {
            _rb.MovePosition(_tr.localPosition +
                             new Vector3(Direction.x * Speed, Direction.y * Speed, Direction.z * Speed));

            if (_pleaseJump && !IsJumping)
            {
                _rb.AddForce(Vector3.up * 200 * Game.Instance.HoleLength);
                IsJumping = true;
                _pleaseJump = false;
            }
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
        IsJumping = false;
        _pleaseJump = false;
        jumpBuffer = 0;
        
        //If human player
        //PlayerController.Instance.Reset();

        _rb.isKinematic = true;
        _tr.localPosition = Vector3.up * 2f;
        CurrentTilePosition = Vector3Int.zero;
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
        _appearSequence?.Kill();

        _appearSequence = DOTween.Sequence()
            .Insert(0, _tr.DOScale(1, 0.6f).SetEase(Ease.OutBack))
            .InsertCallback(0.2f, () => { apearParticleSystem.SetActive(true); })
            .InsertCallback(1f, () =>
            {
                IsRunning = true;
                IsFalling = false;
            });

        _tr.localScale = Vector3.zero;
        IsFalling = true;
        _rb.isKinematic = false;
    }


    public void ChangeDirection(Vector3Int newDirection)
    {
        Direction = newDirection;

        var eulerRotation = Quaternion.LookRotation(Direction, Vector3.up).eulerAngles;
        _rb.DORotate(eulerRotation, 0.4f).SetEase(Ease.OutBack);
    }

    public void Jump()
    {
        _pleaseJump = true;
        jumpBuffer = Game.Instance.HoleLength;
    }

    public void JumpFinished()
    {
        IsJumping = false;
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

        if (CurrentTilePosition != null && tilePosition != CurrentTilePosition)
        {
            FloorManager.Instance.OnPlayerPassedTile();
            
            if (jumpBuffer-- < 0)
            {
                JumpFinished();
            }
        }

        CurrentTilePosition = tilePosition;

        var tile = FloorManager.Instance.PeekTile(CurrentTilePosition.Value);

        if (Game.Instance.IsStarted)
        {
            if (tile == null)
            {
                Game.Instance.PlayerDie();
            }

            if (tile != null)
            {

                if (tile.IsHole)
                {
                    if (!IsJumping)
                    {
                        Game.Instance.PlayerDie();
                    }

                   
                }
            }
        }
    }
}