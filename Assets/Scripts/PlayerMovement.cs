using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement: MonoBehaviour
{
    // Character
    private Rigidbody2D _rigid;


    // Movement
    [Header("Movement")]
    [Tooltip("Move speed of the character")]
    public float moveSpeed = 10f;

    [Tooltip("Magnification of move speed when the character is charging the jump")]
    [Range(0f, 1f)] public float chargingModifyX = 0.4f;

    [Tooltip("Magnification of falling speed when the character is attached to a climbing wall")]
    [Range(0f, 1f)] public float gripModifyY = 0.4f;

    private Vector2 inputDirection;


    // Jump
    [Header("Jump")]
    [Tooltip("Default jump force of the character")]
    public float defaultJumpForce = 15f;

    [Tooltip("Magnification of jump force at maximum charge")]
    [Range(1f, 10f)] public float maxChargeMultiplier = 2f;

    [Tooltip("Charge time required to reach maximum jump force (in seconds)")]
    [Range(0f, 10f)] public float maxChargeTime = 1.5f;

    [Space(10)]
    [Tooltip("Wall jump force of the character")]
    public float wallJumpForce = 20f;

    private InputAction _jumpAction;
    private float jumpChargeTime = 0f;
    private float finalJumpForce = 0f;
    private bool isJumpCharging = false;
    private bool isWallJump = false;


    // Ground Check
    [Header("Ground Check")]
    [Tooltip("Y position gab from position of the character to the center point of the ground check")]
    public float groundCheckOffset = -0.75f;

    [Tooltip("Radius of the ground check.")]
    public float groundCheckRadius = 0.15f;

    [Tooltip("Layer the character uses as the ground")]
    public LayerMask layerGround;

    [Tooltip("Whether the character is on the ground or not")]
    public  bool isGround;


    // Wall Check
    [Header("Wall Check")]
    [Tooltip("X position gab from position of the character to the start point of the climbing wall check")]
    public float wallCheckOffset = 0.75f;

    [Tooltip("Y position gab from the center point to the top point of the character")]
    public float wallCheckPointOffset = 0.75f;

    [Tooltip("Distance to detect the climbing wall")]
    public float wallCheckDistance = 0.1f;

    [Tooltip("Layer the character uses as the climbing wall")]
    public LayerMask layerWall;

    [Tooltip("Whether the character is on the climbing wall or not")]
    public bool isWall;


    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        Jump();
        GroundCheck();
        WallCheck();
    }

    private void FixedUpdate()
    {
        if (isWallJump)
        {
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
    }

    private void WallCheck()
    {
        Vector2 wallCheckDirection = Vector2.right * inputDirection.x;

        Vector2 wallCheckCenterPoint = new Vector2(transform.position.x + (wallCheckOffset * inputDirection.x), transform.position.y);
        Vector2 wallCheckTopPoint = new Vector2(wallCheckCenterPoint.x, transform.position.y + wallCheckPointOffset);
        bool wallCheckCenter = Physics2D.Raycast(wallCheckCenterPoint, wallCheckDirection, wallCheckDistance, layerWall);
        bool wallCheckTop = Physics2D.Raycast(wallCheckTopPoint, wallCheckDirection, wallCheckDistance, layerWall);

        isWall = wallCheckTop && wallCheckCenter;
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
                finalJumpForce = defaultJumpForce * Mathf.Lerp(1f, maxChargeMultiplier, chargeRatio);
                _rigid.linearVelocityY = finalJumpForce;

                isJumpCharging = false;
                jumpChargeTime = 0f;
            }
        }
        else if (isWall)
        {
            if (_jumpAction.WasReleasedThisFrame())
            {
                isWallJump = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Camera"))
        {
            collision.gameObject.GetComponent<CinemachineCamera>().Priority = 1;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Camera"))
        {
            collision.gameObject.GetComponent<CinemachineCamera>().Priority = 0;
            collision.gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
        }
    }
}
