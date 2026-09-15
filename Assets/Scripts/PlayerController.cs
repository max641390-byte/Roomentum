using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed;
    public float walkAcc;
    public float airborneWalkAcc;
    public float JumpForce;
    public float JumpBufferTime;
    public float HangTime;
    public float CoyoteTime;

    [SerializeField] private InputActionReference _move;
    [SerializeField] private InputActionReference _jump;
    [SerializeField] private GroundCheck _groundCheck;

    private Rigidbody2D _rb;
    private float _moveDirection;
    private float _jumpBufferTimer;
    private float _coyoteTimer;
    private float _hangTimer;
    private float _previousVelocityY;

    private void OnEnable()
    {
        _jump.action.started += Jump;
    }

    private void OnDisable()
    {
        _jump.action.started -= Jump;
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _moveDirection = _move.action.ReadValue<float>();
        if (_jumpBufferTimer > 0) _jumpBufferTimer -= Time.deltaTime;

        if (_groundCheck.IsGrounded) _coyoteTimer = CoyoteTime;
        else if (_coyoteTimer > 0) _coyoteTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        float targetSpeed = _moveDirection * MoveSpeed;

        float acceleration = _groundCheck.IsGrounded ? walkAcc : airborneWalkAcc;

        _rb.linearVelocityX = Mathf.MoveTowards(_rb.linearVelocityX, targetSpeed, acceleration * Time.fixedDeltaTime);

        if (_jumpBufferTimer > 0 && _coyoteTimer > 0)
        {
            _rb.AddForceY(JumpForce);

            _jumpBufferTimer = 0;
            _coyoteTimer = 0;
        }

        if (_previousVelocityY > 0 && _rb.linearVelocityY <= 0)
        {
            _hangTimer = HangTime;
        }

        if (_hangTimer > 0)
        {
            _hangTimer -= Time.fixedDeltaTime;
            _rb.linearVelocityY = 0;
        }

        _previousVelocityY = _rb.linearVelocityY;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        _jumpBufferTimer = JumpBufferTime;
    }
}
