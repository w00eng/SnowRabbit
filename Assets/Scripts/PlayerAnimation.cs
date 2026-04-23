using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _anim;
    private Rigidbody2D _rigid;
    private PlayerMovement _pMove;

    private int animIDSpeedX;
    private int animIDSpeedY;
    private int animIDGrounded;
    private int animIDCharge;
    private int animIDMelt;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody2D>();
        _pMove = GetComponent<PlayerMovement>();

        animIDSpeedX = Animator.StringToHash("SpeedX");
        animIDSpeedY = Animator.StringToHash("SpeedY");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDCharge = Animator.StringToHash("Charge");
        animIDMelt = Animator.StringToHash("Melt");
    }

    void Update()
    {
        _anim.SetFloat(animIDSpeedX, _rigid.linearVelocityX);
        _anim.SetFloat(animIDSpeedY, _rigid.linearVelocityY);
        _anim.SetBool(animIDGrounded, _pMove.isGround);
        _anim.SetBool(animIDCharge, _pMove.isJumpCharging);
    }

    public void Melt()
    {
        _anim.SetTrigger(animIDMelt);
    }
}
