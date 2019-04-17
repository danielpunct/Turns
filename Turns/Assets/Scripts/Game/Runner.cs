using DG.Tweening;
using Gamelogic.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Runner : Singleton<Runner>
{
    public enum RunnerState
    {
        Cinematic,
        Running,
        Falling,
        Jumping
        
    }

    public RunnerState State { get; private set; }
    public float playerPresentOffset = 0.7f;
    public GameObject landParticleSystem;
    public GameObject lightLandParticleSystem;
    public Transform graphicHolder;
    public Transform particlesPivot;
    public Vector3Int Direction { get; private set; }
    public Vector3Int? LastTilePosition = Vector3Int.zero;

    public float Speed => (1 / Game.Instance.TilePassTime * Time.fixedDeltaTime);
    
    RunnerModel _currentModel;

    Rigidbody _rb;
    Transform _tr;

    Vector3Int _checkedTilePosition = Vector3Int.up;
    FloorTile _lastSteppedTile;
    float _startDieTime;
    Sequence _seq;

    public Vector3Int TilePosition => new Vector3Int(
        Mathf.RoundToInt(_tr.localPosition.x),
        Mathf.RoundToInt(_tr.localPosition.y),
        Mathf.RoundToInt(_tr.localPosition.z));

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _tr = transform;
    }
    
    Vector3 _dieingForce = Vector3.zero;

    void FixedUpdate()
    {
        if (!Game.Instance.IsStarted)
        {
            return;
        }

        // is falling to it's death for too long
        if (State == RunnerState.Falling)
        {
            if (Time.unscaledTime - _startDieTime > 6)
            {
                return;
            }

            if (Time.unscaledTime - _startDieTime > 0)
            {
                //_rb.AddForce(_dieingForce);
                _rb.MovePosition(_tr.localPosition +
                                 _dieingForce * Speed);
            }
        }

        if (State == RunnerState.Running || State == RunnerState.Jumping)
        {
            _rb.MovePosition(_tr.localPosition +
                             new Vector3(Direction.x * Speed, Direction.y * Speed, Direction.z * Speed));

        }

        if (State != RunnerState.Cinematic)
        {
            CheckTilePosition();
        }

    }

    public void SetModel(RunnerModel model)
    {
        _currentModel = model;
    }

    public void Reset()
    {
        State = RunnerState.Cinematic;
        _currentModel.Reset();

        //If human player
        //PlayerController.Instance.Reset();
        _dieingForce = Vector3.zero;
        _rb.isKinematic = true;
        LastTilePosition = Vector3Int.zero;
        _rb.velocity = Vector3.zero;
        _rb.rotation = Quaternion.identity;
        _tr.rotation = Quaternion.identity;
        Direction = VectorInt.forward;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        landParticleSystem.SetActive(false);
        _tr.localPosition = Vector3.up * 2f;
        _tr.localScale = Vector3.zero;

        _seq?.Kill();
        _seq = DOTween.Sequence()
            .Insert(playerPresentOffset, _tr.DOScale(1, 0.6f).SetEase(Ease.OutBack))
            .InsertCallback(1.5f, _currentModel.SetForMenu);
    }


    public void Play()
    {
        _seq?.Kill();

        _seq = DOTween.Sequence()
            .Insert(0f, _tr.DOScale(1, 0.2f).SetEase(Ease.OutBack))
            .InsertCallback(0.40f, ShowParticles_Land)
            .InsertCallback(1f, () =>
            {
                State = RunnerState.Running;
                _currentModel.SetForRun();
            });

        _tr.localScale = Vector3.zero;
        State = RunnerState.Cinematic;
        _rb.isKinematic = false;
    }

    void ShowParticles_Land()
    {
        landParticleSystem.SetActive(false);
        landParticleSystem.transform.position = particlesPivot.position;
        landParticleSystem.SetActive(true);
    }

    void ShowParticles_LightLand()
    {
        lightLandParticleSystem.SetActive(false);
        lightLandParticleSystem.transform.position = particlesPivot.position;
        lightLandParticleSystem.SetActive(true);
    }

    public void ChangeDirection(Vector3Int newDirection)
    {
        Direction = newDirection;

        if (WasPerfectChange(transform.localPosition, Direction))
        {
            Game.Instance.OnRunnerPerfectChange();
        }

        var eulerRotation = Quaternion.LookRotation(Direction, Vector3.up).eulerAngles;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.DORotate(eulerRotation, 0.4f).SetEase(Ease.OutBack);
    }

    bool WasPerfectChange(Vector3 position, Vector3Int direction)
    {
        if (direction == Vector3Int.left || direction == Vector3Int.right)
        {
            return Mathf.Abs(position.z - Mathf.RoundToInt(position.z)) < Game.Instance.PerfectChangeThreshold;
        }

        return Mathf.Abs(position.x - Mathf.RoundToInt(position.x)) < Game.Instance.PerfectChangeThreshold;
    }


    public void Jump()
    {
        _lifted = false;
        State = RunnerState.Jumping;

        _rb.AddForce(-Game.Instance.DefaultGravity * 8);
    }


    float m_GroundCheckDistance = 0.1f;
    bool _lifted = false;

    bool HasLanded()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * m_GroundCheckDistance));
#endif
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, m_GroundCheckDistance))
        {
            if (!_lifted)
            {
                return false;
            }

            _lifted = false;
            return true;
        }

        _lifted = true;
        return false;
    }

    public void JumpFinished()
    {
        State = RunnerState.Running;
    }

    public void SlowDownAndDie(Vector3Int? awayDirection)
    {
        _startDieTime = Time.unscaledTime;

        _rb.constraints = RigidbodyConstraints.None;
        if (awayDirection != null)
        {
            _dieingForce = awayDirection.Value;
        }

        State = RunnerState.Falling;
    }

    void CheckTilePosition()
    {
        if (HasLanded())
        {
            if (State == RunnerState.Jumping)
            {
                JumpFinished();
            }

            ShowParticles_LightLand();
        }
        
        var tilePosition = TilePosition;

        if (State == RunnerState.Falling || Game.Instance.IsStarted && (object) _lastSteppedTile != null)
        {
            if (tilePosition.y - _lastSteppedTile.PositionKey.y < -1)
            {
                Game.Instance.OnRunnerFallOver();
            }
        }

        if (tilePosition.HorizontalDif(_checkedTilePosition) == Vector3Int.zero)
        {
            return;
        }

        _checkedTilePosition = tilePosition;

        if (LastTilePosition != null && tilePosition != LastTilePosition)
        {
            FloorManager.Instance.OnPlayerPassedTile();
        }

        var tile = FloorManager.Instance.PeekTile(tilePosition);
        if ((object) tile != null)
        {
            _lastSteppedTile = tile;

            if (tile.IsHole && State != RunnerState.Jumping)
            {
                SlowDownAndDie(null);
            }
        }
        else if (State == RunnerState.Running &&
                 LastTilePosition != null &&
                 (object) _lastSteppedTile != null &&
                 _lastSteppedTile.NextPositionKey != null)
        {
            var roadDir = _lastSteppedTile.NextPositionKey.Value.HorizontalDif(_lastSteppedTile.PositionKey);
           if ( roadDir != Direction)

           {
//               _tr.localScale = Vector3.one * 0.5f;
                SlowDownAndDie(tilePosition - _lastSteppedTile.NextPositionKey);
            }

//            var p = tilePosition + Vector3Int.right;
//            if ((object) FloorManager.Instance.PeekTile(p) != null && p != LastTilePosition)
//            {
//                SlowDownAndDie();
////                Game.Instance.PlayerDie(tilePosition - p);
//            }
//            p = tilePosition + Vector3Int.left;
//            if ((object) FloorManager.Instance.PeekTile(p) != null && p != LastTilePosition)
//            {
//                SlowDownAndDie();
////                Game.Instance.PlayerDie(tilePosition - p);
//            }
//            p = tilePosition + VectorInt.forward;
//            if ((object) FloorManager.Instance.PeekTile(p) != null && p != LastTilePosition)
//            {
//                SlowDownAndDie();
////                Game.Instance.PlayerDie(tilePosition - p);
//            }
//            p = tilePosition + VectorInt.back;
//            if ((object) FloorManager.Instance.PeekTile(p) != null && p != LastTilePosition)
//            {
//                SlowDownAndDie();
////                Game.Instance.PlayerDie(tilePosition - p);
//            }
        }


        LastTilePosition = tilePosition;
    }
}