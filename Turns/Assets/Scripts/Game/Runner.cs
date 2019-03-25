using DG.Tweening;
using Gamelogic.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Runner : Singleton<Runner>
{
    public float playerPresentOffset = 0.7f;
    public GameObject apearParticleSystem;
    public GameObject lightLandParticleSystem;
    public Transform graphicHolder;
    public Transform particlesPivot;
    public bool IsFalling { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsJumping { get; private set; }
    public Vector3Int Direction { get; private set; }
    Rigidbody _rb;
    Transform _tr;
    public Vector3Int? CurrentTilePosition = Vector3Int.zero;

    Vector3Int checkedTilePosition = Vector3Int.up;
    FloorTile lastSteppedTile = null;
    float _startDieTime;
    Sequence _seq;

    bool FallingInTheBeginning => IsFalling && Game.Instance.IsStarted;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _tr = transform;
    }

    void FixedUpdate()
    {
        if (!IsRunning && !IsFalling)
        {
            return;
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
            
        }

        if (IsRunning && !IsFalling)
        {
            CheckTilePosition();
        }
    }


    public void Reset()
    {
        IsRunning = false;
        IsFalling = false;
        IsJumping = false;
        
        //If human player
        //PlayerController.Instance.Reset();

        _rb.isKinematic = true;
        CurrentTilePosition = Vector3Int.zero;
        _rb.velocity = Vector3.zero;
        _rb.rotation = Quaternion.identity;
        _tr.rotation = Quaternion.identity;
        Direction = VectorInt.forward;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        apearParticleSystem.SetActive(false);
        _tr.localPosition = Vector3.up * 2f;
        _tr.localScale = Vector3.zero;

        _seq?.Kill();
        _seq = DOTween.Sequence()
            .Insert(playerPresentOffset, _tr.DOScale(1, 0.6f).SetEase(Ease.OutBack));
    }
    
    

    public void Play()
    {
        _seq?.Kill();

        _seq = DOTween.Sequence()
            .Insert(0f, _tr.DOScale(1, 0.2f).SetEase(Ease.OutBack))
            .InsertCallback(0.5f, ShowParticles_Land)
            .InsertCallback(1f, () =>
            {
                IsRunning = true;
                IsFalling = false;
            });

        _tr.localScale = Vector3.zero;
        IsFalling = true;
        _rb.isKinematic = false;
    }

    void ShowParticles_Land()
    {
        apearParticleSystem.SetActive(false);
        apearParticleSystem.transform.position = particlesPivot.position;
        apearParticleSystem.SetActive(true);
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

        var eulerRotation = Quaternion.LookRotation(Direction, Vector3.up).eulerAngles;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.DORotate(eulerRotation, 0.4f).SetEase(Ease.OutBack);
    }

    
    public void Jump()
    {
        lifted = false;
        IsJumping = true;

        _rb.AddForce(-Game.Instance.DefaultGravity * 8);

    }
    
    
    float m_GroundCheckDistance = 0.1f;
    bool lifted = false;
    bool CheckGroundStatus(bool afterStartJump)
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position , transform.position + (Vector3.down * m_GroundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, m_GroundCheckDistance))
        {
            if (afterStartJump && !lifted)
            {
                return false;
            }
        }
        else
        {
            lifted = true;
            return false;
        }

        return true;
    }

    public void JumpFinished()
    {
        IsJumping = false;
        ShowParticles_LightLand();
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
        get { return (1 / Game.Instance.TilePassTime * Time.fixedDeltaTime); }
    }

    void CheckTilePosition()
    {
        if (IsJumping)
        {
            if (CheckGroundStatus(true))
            {
                JumpFinished();
            }
        }
        
        
        var pos = _tr.localPosition;
        
        var tilePosition = new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));

        if (tilePosition.HorizontalDif(checkedTilePosition) == Vector3Int.zero)
        {
            return;
        }

        checkedTilePosition = tilePosition;

        Game.Instance._debug.text = Game.Instance.IsStarted ? tilePosition.ToString() : Game.Instance._debug.text +"end";
        
        if (CurrentTilePosition != null && tilePosition != CurrentTilePosition)
        {
            FloorManager.Instance.OnPlayerPassedTile();
           
        }

        CurrentTilePosition = tilePosition;

        var tile = FloorManager.Instance.PeekTile(CurrentTilePosition.Value);
        if (tile != null)
        {
            lastSteppedTile = tile;
            
            if (tile.IsHole && !IsJumping)
            {
                Game.Instance.PlayerDie();
            }
        }

        if (Game.Instance.IsStarted && lastSteppedTile != null)
        {
            if (tilePosition.y - lastSteppedTile.PositionKey.y < -1)
            {
                Game.Instance.PlayerDie();
            }
        }
    }
}