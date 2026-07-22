using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerData Data;

    #region Variable

    public Rigidbody2D rb {  get; private set; }

    public bool IsFacingRight {  get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsWallJumping { get; private set; }
    public bool IsSliding { get; private set; }

    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }

    private bool _isJumpCut;
    private bool _isJumpFalling;

    private float _wallJumpStartTime;
    private int _lastWallJumpDir;

    private Vector2 _moveInput;
    public float LastPressedJumpTime { get; private set; }

    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f,0.03f);
    [Space(4)]
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f,0.6f);

    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        SetGravityScale(Data.gravityScale);
        IsFacingRight = true;
    }

    private void Update()
    {
        #region TIMERS

        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
        #endregion

        #region Input Handler

        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        if(_moveInput.x != 0)
        {
            CheckDirectionToFace(_moveInput.x > 0);
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.J))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.C) || Input.GetKeyUp(KeyCode.J))
        {
            OnJumpUpInput();
        }
        #endregion

        #region Collision Checks

        if (!IsJumping)
        {
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize,0,_groundLayer) && !IsJumping)
            {
                LastOnGroundTime = Data.coyoteTime;
            }

            bool frontIsRight = _frontWallCheckPoint.position.x > transform.position.x;

            Transform rightPoint = frontIsRight ? _frontWallCheckPoint : _backWallCheckPoint;
            Transform leftPoint = frontIsRight ? _backWallCheckPoint : _frontWallCheckPoint;

            if (Physics2D.OverlapBox(rightPoint.position, _wallCheckSize, 0, _groundLayer) && !IsWallJumping) 
            {
                LastOnWallRightTime = Data.coyoteTime;
            }

            if (Physics2D.OverlapBox(leftPoint.position, _wallCheckSize, 0, _groundLayer) && !IsWallJumping)
            {
                LastOnWallLeftTime = Data.coyoteTime;
            }

            LastOnWallTime = Mathf.Max(LastOnWallRightTime,LastOnWallLeftTime);
        }
        #endregion

        #region Jump Checks

        if (IsJumping && rb.linearVelocity.y < 0)
        {
            IsJumping = false;

            if (!IsWallJumping)
            {
                _isJumpFalling = true;
            }
        }

        if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
        {
            IsWallJumping = false;
        }

        if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            _isJumpCut = false;

            if (!IsJumping)
            {
                _isJumpFalling = false;
            }
        }

        if (CanJump() && LastPressedJumpTime > 0)
        {
            IsJumping = true;
            IsWallJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            Jump();
        }

        else if (CanWallJump() && LastPressedJumpTime > 0)
        {
            IsWallJumping = true;
            IsJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            _wallJumpStartTime = Time.time;
            _lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

            WallJump(_lastWallJumpDir);
        }
        #endregion

        #region Slide Checks

        if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
        {
            IsSliding = true;
        }
        else
        {
            IsSliding = false;
        }
        #endregion

        #region Gravity

        if (IsSliding)
        {
            SetGravityScale(0);
        }
        else if(rb.linearVelocity.y < 0 && _moveInput.y < 0)
        {
            SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
        }
        else if (_isJumpCut)
        {
            SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, - Data.maxFallSpeed));
        }
        else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(rb.linearVelocity.y) < Data.jumpHangTimeThreshold)
        {
            SetGravityScale(Data.gravityScale * Data.JumpHangGravityMult);
        }
        else if (rb.linearVelocity.y < 0)
        {
            SetGravityScale(Data.gravityScale * Data.fallGravityMult);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -Data.maxFallSpeed));
        }
        else
        {
            SetGravityScale(Data.gravityScale);
        }
        #endregion
    }

    private void FixedUpdate()
    {
        if (IsWallJumping)
        {
            Run(Data.wallJumpRunLerp);
        }
        else
        {
            Run(1);
        }

        if (IsSliding)
        {
            Slide();
        }
    }

    #region Input CallBack

    public void OnJumpInput()
    {
        LastPressedJumpTime = Data.JumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if(CanJumpCut() || CanWallJumpCut())
        {
            _isJumpCut = true;
        }
    }

    #endregion

    #region General Methods

    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

    #endregion

    #region Run Methods

    private void Run(float lerpAmount)
    {
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;
        targetSpeed = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate

        float accelRate;

        bool hasInput = Mathf.Abs(_moveInput.x) > 0.01f;

        if (LastOnGroundTime > 0)
        {
            accelRate = hasInput ? Data.runAccelAmount : Data.runDeccelAmount;
        }
        else
        {
            accelRate = hasInput ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        }
        #endregion

        #region Add Bonus Jump Apex Acceleration

        if((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(rb.linearVelocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }

        #endregion

        #region Conserve Momentum

        if (Data.doConserveMomentum && Mathf.Abs(rb.linearVelocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            accelRate = 0;
        }

        #endregion

        float speedDif = targetSpeed - rb.linearVelocity.x;
        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }

    #endregion

    #region Jump Methods

    private void Jump()
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        #region Perform Jump

        float force = Data.jumpForce;
        if(rb.linearVelocity.y < 0)
        {
            force -= rb.linearVelocity.y;
        }
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        #endregion
    }

    private void WallJump(int dir)
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        #region Perform Wall Jump
        rb.linearVelocity = Vector2.zero;

        Vector2 force = new Vector2(Data.wallJumpForce.x * dir, Data.wallJumpForce.y);

        rb.AddForce(force, ForceMode2D.Impulse);

        if (Data.doTurnOnWallJump)
        {
            CheckDirectionToFace(dir > 0);
        }

        #endregion
    }

    #endregion

    #region Other Movement Methods
    private void Slide()
    {
        float speedDif = Data.slideSpeed - rb.linearVelocity.y;
        float movement = speedDif * Data.slideAccel;

        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1/Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1/Time.fixedDeltaTime));

        rb.AddForce(movement * Vector2.up);
    }

    #endregion

    #region Check Methods

    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
        {
            Turn();
        }
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;
    }

    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 &&
            (!IsWallJumping || (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
    }

    private bool CanJumpCut()
    {
        return IsJumping && rb.linearVelocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return IsWallJumping && rb.linearVelocity.y > 0;
    }

    public bool CanSlide()
    {
        if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && LastOnGroundTime <=0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region Editor Methods

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }

    #endregion

}
