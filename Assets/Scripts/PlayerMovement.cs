using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement: MonoBehaviour
{
    // Character
    private Rigidbody2D _rigid;
    private SpriteRenderer _render;
    private PlayerAnimation _pAnim;

    public Vector2 spawnPoint = Vector2.zero;
    public static bool playerPause = false;


    // Movement
    [Header("Movement")]
    [Tooltip("Move speed of the character")]
    [Min(0f)] public float moveSpeed = 10f;

    [Tooltip("Magnification of move speed when the character is charging the jump")]
    [Range(0f, 1f)] public float chargingModifyX = 0.4f;

    [Tooltip("Magnification of falling speed when the character is attached to the climbing wall")]
    [Range(0f, 1f)] public float gripModifyY = 0.4f;

    [Space(7)]
    [Tooltip("Time when the character can be stopped (in seconds)")]
    [Min(0f)] public float maxStopTime = 5f;

    [Tooltip("Time the character stopped (in seconds)")]
    public float stopTime = 0f;

    private Vector2 inputDirection;


    // Jump
    [Header("Jump")]
    [Tooltip("Default jump force of the character")]
    [Min(0f)] public float defaultJumpForce = 13f;

    [Tooltip("Magnification of jump force at maximum charge")]
    [Min(0f)] public float maxChargeMultiplier = 2f;

    [Tooltip("Charge time required to reach maximum jump force (in seconds)")]
    [Min(0f)] public float maxChargeTime = 1.5f;

    [Tooltip("Time the character was charging (in seconds) (Maximum value: maxChargeTime)")]
    public float jumpChargeTime = 0f;

    [Tooltip("Whether the character is charging the jump or not")]
    public bool isJumpCharging = false;

    [Space(7)]
    [Tooltip("Wall jump force of the character")]
    [Min(0f)] public float wallJumpForce = 30f;

    [Tooltip("Whether the character does wall jump or not")]
    public bool isWallJump = false;

    [Space(7)]
    [Tooltip("Jump force of the character, when character is on the mushroom")]
    [Min(0f)] public float mushroomJumpForce = 30f;

    private InputAction _jumpAction;


    // Ground Check
    [Header("Ground Check")]
    [Tooltip("Y position gab from position of the character to the center point of the ground check")]
    public float groundCheckOffset = -0.7f;

    [Tooltip("Radius of the ground check.")]
    [Min(0.00001f)] public float groundCheckRadius = 0.3f;

    [Space(7)]
    [Tooltip("Layer the character uses as the ground")]
    public LayerMask layerGround;

    [Tooltip("Whether the character is on the ground or not")]
    public bool isGround;

    [Space(7)]
    [Tooltip("Layer the character uses as the mushroom")]
    public LayerMask layerMushroom;

    [Tooltip("Whether the character is on the mushroom or not")]
    public bool isMushroom;


    // Wall Check
    [Header("Wall Check")]
    [Tooltip("X position gab from position of the character to the start point of the climbing wall check")]
    public float wallCheckOffsetX = 0.6f;

    [Tooltip("Y position gab from position of the character to the start point of the climbing wall check")]
    public float wallCheckOffsetY = -0.265f;

    [Tooltip("Y position gab from the center point to the top point of the character")]
    public float wallCheckPointOffset = 0.6f;

    [Tooltip("Distance to detect the climbing wall")]
    [Min(0f)] public float wallCheckDistance = 0.1f;

    [Tooltip("Layer the character uses as the climbing wall")]
    public LayerMask layerWall;

    [Tooltip("Whether the character is on the climbing wall or not")]
    public bool isWall;

    [Space(7)]
    [Tooltip("Time when the character can be stopped on the climbing wall (in seconds)")]
    [Min(0f)] public float maxWallStopTime = 0.5f;

    [Tooltip("Time the character stopped on the climbing wall (in seconds) (Maximum value: maxWallStopTime)")]
    public float wallStopTime = 0f;

    [Tooltip("Whether the character is stopped on the climbing wall or not")]
    public bool isWallStop;



    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _render = GetComponent<SpriteRenderer>();
        _pAnim = GetComponent<PlayerAnimation>();
        _jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        if (playerPause)
        {
            return;
        }

        if (_rigid.linearVelocity == Vector2.zero)
        {
            stopTime += Time.deltaTime;
            if (stopTime >= maxStopTime)
            {
                _pAnim.Melt();
                stopTime = 0f;
            }
        }
        else
        {
            stopTime = 0f;
        }

            GroundCheck();
        WallCheck();
        Jump();

        if (isWallStop)
        {
            WallStop();
        }
    }

    private void FixedUpdate()
    {
        if (playerPause)
        {
            return;
        }

        _render.flipX = (inputDirection.x == 0f) ? _render.flipX : (inputDirection.x < 0f);

        if (isWallJump)
        {
            _rigid.gravityScale = 1f;
            _rigid.linearVelocity = Vector2.zero;
            _rigid.linearVelocity = new Vector2(-inputDirection.x * wallJumpForce, wallJumpForce);

            isWallJump = false;
        }
        else
        {
            float moveModifyX = isJumpCharging ? (moveSpeed * chargingModifyX) : moveSpeed;
            float moveModifyY = isWall ? gripModifyY : 1f;

            _rigid.linearVelocity = new Vector2(inputDirection.x * moveModifyX, _rigid.linearVelocity.y * moveModifyY);
        }
    }

    private void GroundCheck()
    {
        Vector2 groundCheckPoint = new Vector2(transform.position.x, transform.position.y + groundCheckOffset);
        isGround = Physics2D.OverlapCircle(groundCheckPoint, groundCheckRadius, layerGround);
        isMushroom = Physics2D.OverlapCircle(groundCheckPoint, groundCheckRadius, layerMushroom);
    }

    private void WallCheck()
    {
        Vector2 wallCheckDirection = Vector2.right * inputDirection.x;

        Vector2 wallCheckCenterPoint = new Vector2(transform.position.x + (wallCheckOffsetX * inputDirection.x), transform.position.y + wallCheckOffsetY);
        Vector2 wallCheckTopPoint = new Vector2(wallCheckCenterPoint.x, wallCheckCenterPoint.y + wallCheckPointOffset);
        bool wallCheckCenter = Physics2D.Raycast(wallCheckCenterPoint, wallCheckDirection, wallCheckDistance, layerWall);
        bool wallCheckTop = Physics2D.Raycast(wallCheckTopPoint, wallCheckDirection, wallCheckDistance, layerWall);

        isWall = wallCheckTop && wallCheckCenter && !isGround;
    }

    public void OnMove(InputValue value)
    {
        inputDirection = value.Get<Vector2>();
    }

    private void Jump()
    {
        if (isGround)
        {
            if (_jumpAction.IsPressed())
            {
                isJumpCharging = true;

                jumpChargeTime += Time.deltaTime;
                jumpChargeTime = Mathf.Clamp(jumpChargeTime, 0f, maxChargeTime);
            }
            if (_jumpAction.WasReleasedThisFrame())
            {
                float chargeRatio = jumpChargeTime / maxChargeTime;
                float finalJumpForce = defaultJumpForce * Mathf.Lerp(1f, maxChargeMultiplier, chargeRatio);
                _rigid.linearVelocityY = finalJumpForce;

                isJumpCharging = false;
                jumpChargeTime = 0f;
            }
        }
        else
        {
            isJumpCharging = false;
            jumpChargeTime = 0f;
        }

        if (isMushroom)
        {
            _rigid.linearVelocityY = mushroomJumpForce;
        }
        
        if (isWall)
        {
            if (_jumpAction.WasReleasedThisFrame())
            {
                isWallJump = true;
            }
        }
    }

    private void WallStop()
    {
        wallStopTime += Time.deltaTime;
        _rigid.gravityScale = 0f;

        if (wallStopTime >= maxWallStopTime || !isWall)
        {
            isWallStop = false;
            _rigid.gravityScale = 1f;
            wallStopTime = 0f;
        }
    }

    public void PlayerPauseOn()
    {
        playerPause = true;
        _rigid.linearVelocity = Vector2.zero;
        _rigid.gravityScale = 0f;
    }

    public void PlayerPauseOff()
    {
        playerPause = false;
        _rigid.gravityScale = 1f;
    }

    public void Respawn()
    {
        transform.position = spawnPoint;
    }
}
