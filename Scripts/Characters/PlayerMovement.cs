using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerData Data;

    #region Variable

    public Rigidbody2D rb {  get; private set; }
    public SpriteRenderer sr {  get; private set; }
    public TrailRenderer tr { get; private set; }
    public CircleCollider2D cl { get; private set; }
    private Viewer viewer = null;

    public bool IsFacingRight {  get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsWallJumping { get; private set; }
    public bool IsSliding { get; private set; }
    public bool IsGroundSliding { get; private set; }

    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }
    public float LastOnGlidePressed {  get; private set; }

    private float _originalRadius = 0.4f;
    private float _originalYOffset = -0.1f;

    private bool _isJumpCut;
    private bool _isJumpFalling;

    private bool _isDashing;
    public bool _canDash = true;
    private bool _isGliding = false;

    private bool _active = true;
    private bool _bright = false;

    private float _wallJumpStartTime;
    private int _lastWallJumpDir;

    private Vector2 _moveInput;
    private Vector2 _dashDir;
    private Vector2 _respawnPos;

    private Color originalColor;
    private Coroutine _groundSlideCoroutine;

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
        sr = GetComponent<SpriteRenderer>();
        tr = GetComponent<TrailRenderer>();
        cl = GetComponent<CircleCollider2D>();

        SetRespawnPoint(transform.position);
        originalColor = sr.color;
    }

    private void Start()
    {
        SetGravityScale(Data.gravityScale);
        IsFacingRight = true;
        IsGroundSliding = false;
    }

    private void Update()
    {

        if (!_active)
        {
            if (viewer != null) { 
                if (viewer.active == false)
                {
                    CameraManager.instance.ChangeTarget(transform);
                    _active = true;
                }
            }
            return;
        }
        #region TIMERS

        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
        LastOnGlidePressed -= Time.deltaTime;
        #endregion

        #region Input Handler

        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");
        var dashInput = Input.GetKeyDown(KeyCode.LeftShift);

        if(_moveInput.x != 0)
        {
            CheckDirectionToFace(_moveInput.x > 0);
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.J)) && !Input.GetKey(KeyCode.S))
        {
            OnJumpInput();
        }

        if ((Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.C) || Input.GetKeyUp(KeyCode.J)) && !Input.GetKey(KeyCode.S))
        {           
            OnJumpUpInput();
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.J)) && Input.GetKey(KeyCode.S) && Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsGroundSliding && !_isDashing && _canDash)
        {
            if (sr.color == Color.yellow)
            {
                sr.color = originalColor;
            }

            IsGroundSliding = true;
            if (_groundSlideCoroutine != null)
            {
                StopCoroutine(_groundSlideCoroutine);
            }
            _groundSlideCoroutine = StartCoroutine(StopGroundSliding());
        }

        if (((Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.C) || Input.GetKeyUp(KeyCode.J) || Input.GetKeyUp(KeyCode.S)) && IsGroundSliding) || !Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
        {
            CancelSlide();
        }

        if (IsJumping && Input.GetKeyDown(KeyCode.V))
        {
            LastOnGlidePressed = Data.gliderInputBuffer;
        }
        if (IsJumping && Input.GetKeyUp(KeyCode.V))
        {
            LastOnGlidePressed -= Data.gliderInputBuffer;
        }

        if (_isJumpFalling && Input.GetKeyDown(KeyCode.V))
        {
            _isGliding = true;
        }

        if (_isGliding && Input.GetKeyUp(KeyCode.V))
        {
            _isGliding = false;
        }

        if (dashInput && _canDash)
        {
            _isDashing = true;
            _canDash = false;
            tr.emitting = true;
            _dashDir = new Vector2(_moveInput.x, _moveInput.y);

            if (_dashDir == Vector2.zero)
            {
                _dashDir = new Vector2(transform.localScale.x, 0);
            }
            StartCoroutine(StopDashing());
        }

        if (viewer != null && Input.GetKeyDown(KeyCode.E))
        {
            viewer.StartViewing();
            rb.linearVelocity = Vector2.zero;
            _active = false;
        }

        
        #endregion

        #region Collision Checks

        if (!IsJumping)
        {
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize,0,_groundLayer) && !IsJumping)
            {
                LastOnGroundTime = Data.coyoteTime;
                if (!_isDashing)
                {
                    _canDash = true;
                }
                if (_isGliding == true)
                {
                    _isGliding = false;
                }
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
            if (LastOnGlidePressed > 0)
            {
                _isGliding = true;
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
        else if (_isGliding)
        {
            SetGravityScale(Data.gravityScale * Data.gliderGravityMult);
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

        #region Coloring

        if (_bright)
        {
            _bright = ImpactFlash.instance.bright;
            return;
        }
        else if (!_canDash || _isDashing)
        {
            sr.color = Color.yellow;
        }
        else if (IsGroundSliding)
        {
            sr.color = Color.blueViolet;
        }
        else
        {
            sr.color = originalColor;
        }

        #endregion
    }

    private void FixedUpdate()
    {
        if (!_active)
        {
            return;
        }

        if (_isDashing)
        {
            if (IsGroundSliding)
            {
                CancelSlide();
            }
            Dash();
        }
        else if (IsGroundSliding)
        {
            GroundSlide();
        }
        else if (IsWallJumping)
        {
            Run(Data.wallJumpRunLerp);
        }
        else
        {
            Run(1);
        }
        
        if (IsSliding && !_isDashing)
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -1f);
            }
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
        _bright = true;
        ImpactFlash.instance.Flash(sr, 0.15f, Color.white);

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
        
        float targetSpeed = -Mathf.Abs(Data.slideSpeed);
        float yVelocity = Mathf.MoveTowards(rb.linearVelocity.y,targetSpeed, Data.slideAccel * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, yVelocity);
    }

    private void Dash()
    {
        rb.linearVelocity = _dashDir.normalized * Data.dashVelocity;
    }

    private void GroundSlide()
    {
        cl.radius = 0.25f;
        cl.offset = new Vector2(0, -0.25f);
        

        float direction = IsFacingRight ? 1 : -1;
        float xVelocity = Mathf.MoveTowards(rb.linearVelocity.x, Data.groundSlideSpeed * direction, Data.groundSlideAccel * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(xVelocity, rb.linearVelocity.y);
    }
    private void CancelSlide()
    {
        IsGroundSliding = false;
        cl.radius = _originalRadius;
        cl.offset = new Vector2(0, _originalYOffset);

        if (_groundSlideCoroutine != null)
        {
            StopCoroutine(_groundSlideCoroutine);
            _groundSlideCoroutine = null;
        }
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(Data.dashTime);
        tr.emitting = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, -0.1f);
        _isDashing = false;

    }

    private IEnumerator StopGroundSliding()
    {
        yield return new WaitForSeconds(Data.groundSlideTime);
        CancelSlide();

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Viewer")
        {
            viewer = other.GetComponent<Viewer>();
            
        }

        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Viewer")
        {
            viewer = null;
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

    #region Life Control Methods

    public void Die()
    {
        _active = false;
        cl.enabled = false;
        StartCoroutine(Respawn());
    }

    public void SetRespawnPoint(Vector2 position)
    {
        _respawnPos = position;
    }

    private IEnumerator Respawn()
    {
        CameraManager.instance.BasicScreenShake(Data.deathCameraShakeIntensity, Data.deathCameraShakeFrequency,Data.deathCameraShakeDuration);
        DeathTransition.instance.SetTransition(true);
        sr.enabled = false;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1.3f);
        SetGravityScale(0);
        cl.enabled = true;
        transform.position = _respawnPos;
        CameraManager.instance.NewDeath();
        yield return new WaitForSeconds(1f);
        SetGravityScale(Data.gravityScale);
        CameraManager.instance.ChangeTarget(transform);
        DeathTransition.instance.SetTransition(false);
        sr.enabled = true;
        yield return new WaitForSeconds(0.2f);

        _canDash = true;
        _active = true;
        
    }

    #endregion

}
