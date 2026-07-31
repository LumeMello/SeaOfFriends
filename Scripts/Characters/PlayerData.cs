using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "new PlayerData", menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("SpriteManipulation")]
    [Range(0f, 1f)] public float timeToStretch;
    [Range(0f, 1f)] public float timeToSquash;
    [Range(0f, 1.5f)] public float scaleToStretch_x;
    [Range(0f, 1.5f)] public float scaleToStretch_y;
    [Range(0f, 1.5f)] public float scaleToSquash_x;
    [Range(0f, 1.5f)] public float scaleToSquash_y;

    [Space(20)]

    [Header("Gravity")]
    public float fallGravityMult;
    public float maxFallSpeed;
    [Space(4)]
    public float fastFallGravityMult;
    public float maxFastFallSpeed;
    [Space(4)]
    [HideInInspector] public float gravityStrength;
    [HideInInspector] public float gravityScale;

    [Space(20)]

    [Header("Run")]
    public float runMaxSpeed;
    public float runAcceleration;
    [HideInInspector] public float runAccelAmount;
    public float runDecceleration;
    [HideInInspector] public float runDeccelAmount;
    [Space(4)]
    [Range(0f, 1f)] public float accelInAir;
    [Range(0f, 1f)] public float deccelInAir;
    [Space(4)]
    public bool doConserveMomentum = true;

    [Space(20)]
    [Header("Jump")]
    public float jumpHeight;
    public float jumpTimeToApex;
    [HideInInspector] public float jumpForce;

    [Header("Both Jumps")]
    public float jumpCutGravityMult;
    [Range(0f, 1f)] public float JumpHangGravityMult;
    public float jumpHangTimeThreshold;
    [Space(0.5f)]
    public float jumpHangAccelerationMult;
    public float jumpHangMaxSpeedMult;

    [Header("Wall Jump")]
    public Vector2 wallJumpForce;
    [Space(4)]
    [Range(0f, 1f)] public float wallJumpRunLerp;
    [Range(0f, 1.5f)] public float wallJumpTime;
    public bool doTurnOnWallJump;

    [Space(20)]

    [Header("Sliders")]
    public float slideSpeed;
    public float slideAccel;
    public float groundSlideSpeed;
    public float groundSlideAccel;
    [Range(0.01f, 1f)] public float groundSlideTime;

    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime;
    [Range(0.01f, 0.5f)] public float JumpInputBufferTime;

    [Space(20)]

    [Header("Death Camera")]
    [Range(0.01f, 5f)] public float deathCameraShakeIntensity;
    [Range(0.01f, 1f)] public float deathCameraShakeFrequency;
    [Range(0.01f, 1f)] public float deathCameraShakeDuration;

    [Space(20)]

    [Header("Dash")]
    public float dashVelocity;
    [Range(0.01f, 1f)] public float dashTime;

    [Header("Glider")]
    [Range(0.01f, 1f)] public float gliderGravityMult;
    [Range(0.01f, 1f)] public float gliderInputBuffer;

    private void OnValidate()
    {
        gravityStrength = -(2* jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        gravityScale = gravityStrength / Physics2D.gravity.y;

        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
    }

}
