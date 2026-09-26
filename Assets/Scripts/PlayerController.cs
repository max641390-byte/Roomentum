using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Extra")]
    public Transform RespawnPoint;

    //Public
    [Header("Movement")]
    public float MoveSpeed;
    public float walkAcc;
    public float airborneWalkAcc;
    public float JumpForce;
    public float JumpBufferTime;
    public float HangTime;
    public float GroundStickForce;
    public float JumpCutMultiplier;
    public float MaxFallSpeed;
    public float CoyoteTime;

    [Header("Gravity")]
    public float Gravity;
    public float FallGravity;

    [Header("Abilities")]
    public float DashSpeed;
    public float DashDuration;
    public float DashCooldown;
    public float DashBoost;
    public float NaturalDashBoostDecay;
    public float AgainstDashBoostDecay;
    public float WithDashBoostDecay;
    public float NaturalAttackBoostDecay;
    public float AgainstAttackBoostDecay;
    public float WithAttackBoostDecay;
    public float WallJumpForce;
    public float WallJumpHorizontalForce;
    public float GliderGravityMultiplier;
    public float AttackBoost;
    public float AttackDuration;
    public float AttackCooldown;
    public float SprintSpeedMultiplier;
    public float SlideSpeedMultiplier;

    //Private refs
    [Header("References")]
    [SerializeField] private InputActionReference _move;
    [SerializeField] private InputActionReference _jump;
    [SerializeField] private InputActionReference _dash;
    [SerializeField] private InputActionReference _slide;
    [SerializeField] private InputActionReference _attack;
    [SerializeField] private GameObject _playerHitbox;
    [SerializeField] private GameObject _attackHitbox;
    [SerializeField] private GameObject _slideHitbox;
    [SerializeField] private GroundCheck _groundCheck;
    [SerializeField] private Wallcheck _wallCheck;
    [SerializeField] private AttackCheck _attackCheck;
    [SerializeField] private LayerMask _solidLayers;
    [SerializeField] private AudioClip jumpSound, doubleJumpSound, dashSound, slideSound, attackSound, powerupSound,noteSound, flagSound, playerDeathSound; // Lukas

    //Private
    private Rigidbody2D _rb;
    private PlayerAbilities _abilities;
    private AudioSource audioSource;

    private float _moveDirection;
    private float _facingDirection;
    private float _jumpBufferTimer;
    private float _coyoteTimer;
    private float _hangTimer;
    private float _previousVelocityY;
    private bool _doubleJumpUsed;
    private bool _dashUsed;
    private float _dashTimer;
    private float _dashCooldownTimer;
    private Vector2 _dashDirection;
    private bool _isDashing;
    private bool _jumpHeld;
    private bool _dashJumpBuffered;
    private bool _isTouchingWall;
    private float _wallDirection;
    private bool _rdyWalljump; //LUKAS
    private bool _isSliding;
    private float _slideDirection;
    private bool _isAttacking;
    private float _attackTimer;
    private float _attackCooldownTimer;
    private float _attackHitboxOffsetX;
    private float _attackDirection;
    private bool _attackBoosted;

    //Velocity
    private Vector2 _walkVelocity;
    private Vector2 _dashVelocity;
    private Vector2 _jumpVelocity;
    private Vector2 _slideVelocity;
    private Vector2 _dashBoostVelocity;
    private Vector2 _attackBoostVelocity;

    //Animator
    private Animator anim; //LUKAS
    private SpriteRenderer rend;

    private void OnEnable()
    {
        _jump.action.started += Jump;
        _dash.action.started += Dash;
        _attack.action.started += Attack;
    }
    private void OnDisable()
    {
        _jump.action.started -= Jump;
        _dash.action.started -= Dash;
        _attack.action.started -= Attack;
    }
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _abilities = GetComponent<PlayerAbilities>();

        _attackHitboxOffsetX = Mathf.Abs(_attackHitbox.transform.localPosition.x);
        _facingDirection = transform.localScale.x >= 0 ? 1f : -1f;
    }
    void Start()
    {
        anim = GetComponent <Animator>(); //LUKAS
        rend = GetComponent<SpriteRenderer>();//Lukas
        audioSource = GetComponent<AudioSource>(); //Lukas
        _slideVelocity = Vector2.zero;
        _walkVelocity = Vector2.zero;
        _dashVelocity = Vector2.zero;
        _jumpVelocity = Vector2.zero;
        _isDashing = false;
        _isSliding = false;
        _rdyWalljump = false; //LUKAS
        _playerHitbox.SetActive(true);
        _slideHitbox.SetActive(false);
    }
    void Update()
    {
        _moveDirection = _move.action.ReadValue<Vector2>().x;
        _jumpHeld = _jump.action.IsPressed();

        anim.SetFloat("MoveSpeed",Mathf.Abs (_rb.linearVelocity.x)); //LUKAS
        anim.SetFloat("VerticalSpeed", (_rb.linearVelocityY));
        anim.SetBool("IsGrounded", _groundCheck.IsGrounded);
        anim.SetBool("IsDashing", _isDashing);
        anim.SetBool("IsSliding", _isSliding);
        anim.SetBool("IsAttacking", _isAttacking);
        anim.SetBool("WalljumpRdy",_rdyWalljump);
        //Debug.Log(_rb.linearVelocityY.ToString());


     


                CheckWall();

        if (_facingDirection < 0f) //LUKAS
        {
            FlipSprite(true);
        }
        if (_facingDirection > 0f) //LUKAS
        {
            FlipSprite(false);
        }

        if (_jumpBufferTimer > 0) _jumpBufferTimer -= Time.deltaTime;
        if (_dashCooldownTimer > 0) _dashCooldownTimer -= Time.deltaTime;
        if (_moveDirection != 0) _facingDirection = Mathf.Sign(_moveDirection);

        if (_groundCheck.IsGrounded)
        {
            _coyoteTimer = CoyoteTime;

            _doubleJumpUsed = false;
            _dashUsed = false;
        }
        else if (_coyoteTimer > 0) _coyoteTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleDash();
        HandleSlide();
        HandleJump();
        HandleGravity();
        HandleAttack();

        HandleVelocityCalculation();

    }
    void OnTriggerEnter2D(Collider2D other) // powerup sound fx
    {
        if (other.CompareTag("Powerup"))
            audioSource.PlayOneShot(powerupSound, 0.3f);
        if (other.CompareTag("Note"))
            audioSource.PlayOneShot(noteSound, 0.3f);
        if (other.CompareTag("Flag"))
            audioSource.PlayOneShot(flagSound, 0.3f);
        if (other.CompareTag("Killzone"))
            audioSource.PlayOneShot(playerDeathSound, 0.3f);
    }
    private void FlipSprite(bool direction) //LUKAS
    {
        rend.flipX = direction; 

    }
    private void HandleMovement()
    {
        if (_isDashing || _isSliding)
        {
            _walkVelocity = Vector2.zero;
            return;
        }

        float speed = _abilities.CanSprint ? MoveSpeed * SprintSpeedMultiplier : MoveSpeed;
        _walkVelocity.x = Mathf.MoveTowards(_walkVelocity.x, _moveDirection * speed, (_groundCheck.IsGrounded ? walkAcc : airborneWalkAcc) * Time.fixedDeltaTime);
    }
    private void HandleDash()
    {
        if (_isDashing)
        {
            _dashTimer -= Time.fixedDeltaTime;
            _dashVelocity = _dashDirection * DashSpeed;
            _jumpVelocity.y = 0f;
            

            if (_dashTimer <= 0)
            {
                _isDashing = false;
                _dashVelocity = Vector2.zero;

                _jumpVelocity.y = 0f;

                _dashBoostVelocity = new Vector2(_dashDirection.x * DashBoost, 0f);
            }

            return;
        }

        _dashVelocity = Vector2.zero;

        float decay = NaturalDashBoostDecay;

        if (_moveDirection != 0)
        {
            if (Mathf.Sign(_moveDirection) == Mathf.Sign(_dashBoostVelocity.x))
            {
                decay = WithDashBoostDecay;
            }
            else
            {
                decay = AgainstDashBoostDecay;
            }
        }

        _dashBoostVelocity.x = Mathf.MoveTowards(_dashBoostVelocity.x, 0f, decay * Time.fixedDeltaTime);

        _dashBoostVelocity.y = 0f;
    }
    private void HandleJump()
    {
        if (_isDashing || _isSliding) return;

        bool jumped = false;
        _rdyWalljump = false;

        if (_dashJumpBuffered)
        {
            if (_coyoteTimer > 0)
            {
                _jumpVelocity.y = JumpForce;
                

                _coyoteTimer = 0;
                jumped = true;
            }
            else if (_abilities.CanDoubleJump && !_doubleJumpUsed)
            {
                _hangTimer = 0f;
                _jumpVelocity.y = JumpForce;
                audioSource.PlayOneShot(doubleJumpSound, 0.3f);

                _doubleJumpUsed = true;
                jumped = true;
            }

            _dashJumpBuffered = false;
            _jumpBufferTimer = 0f;
        }
        else if (_jumpBufferTimer > 0 && _coyoteTimer > 0)
        {
            _jumpVelocity.y = JumpForce;

            _jumpBufferTimer = 0;
            _coyoteTimer = 0;
            audioSource.PlayOneShot(jumpSound, 0.3f); //LUKAS

            jumped = true;
        }
        else if (_jumpBufferTimer > 0 && _isTouchingWall && _abilities.CanWallJump)
        {
            _rdyWalljump = true; // lukas
            _jumpVelocity.y = WallJumpForce;
            _walkVelocity.x = -_wallDirection * WallJumpHorizontalForce;

            _jumpBufferTimer = 0;
            _hangTimer = 0f;

            _dashUsed = false;
            _dashCooldownTimer = 0f;

            jumped = true;
            
        }
        else if (_jumpBufferTimer > 0 && _abilities.CanDoubleJump && !_doubleJumpUsed)
        {
            _hangTimer = 0f;
            _jumpVelocity.y = JumpForce;

            _jumpBufferTimer = 0;
            _doubleJumpUsed = true;
            audioSource.PlayOneShot(doubleJumpSound, 0.3f);

            jumped = true;
        }

        if (!jumped && _previousVelocityY > 0 && _jumpVelocity.y <= 0)
        {
            _hangTimer = HangTime;
        }

        if (_hangTimer > 0)
        {
            _hangTimer -= Time.fixedDeltaTime;
            _jumpVelocity.y = 0;
        }

        _previousVelocityY = _jumpVelocity.y;
    }
    private void HandleGravity()
    {
        if (_groundCheck.IsGrounded && _jumpVelocity.y <= 0)
        {
            _jumpVelocity.y = -GroundStickForce;
            return;
        }

        if (_jumpVelocity.y > 0)
        {
            _jumpVelocity.y -= Gravity * Time.fixedDeltaTime;
        }
        else if (_jumpHeld && _abilities.CanGlide)
        {
            _jumpVelocity.y -= FallGravity * GliderGravityMultiplier * Time.fixedDeltaTime;
        }
        else
        {
            _jumpVelocity.y -= FallGravity * Time.fixedDeltaTime;
        }

        _jumpVelocity.y = Mathf.Max(_jumpVelocity.y, -MaxFallSpeed);

        if (!_jumpHeld && _jumpVelocity.y > 0)
        {
            _jumpVelocity.y -= Gravity * (JumpCutMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
    public void HandleAttack()
    {
        _attackHitbox.transform.localPosition = new Vector3(_attackHitboxOffsetX * _attackDirection, _attackHitbox.transform.localPosition.y, _attackHitbox.transform.localPosition.z);

        if (_isAttacking)
        {
            _attackTimer -= Time.fixedDeltaTime;

            if (_attackCheck.IsTouchingObject && !_attackBoosted)
            {
                _attackBoostVelocity.x = -_attackCheck.ObjectDirection * AttackBoost;
                _attackBoosted = true;
            }

            if (_attackTimer <= 0)
            {
                _isAttacking = false;
                _attackCooldownTimer = AttackCooldown;
                _attackBoosted = false;
            }
        }
        else
        {
            _attackHitbox.SetActive(false);
        }

        if (_attackCooldownTimer > 0) _attackCooldownTimer -= Time.fixedDeltaTime;

        float attackDecay = NaturalAttackBoostDecay;

        if (_moveDirection != 0)
        {
            if (Mathf.Sign(_moveDirection) == Mathf.Sign(_attackBoostVelocity.x))
            {
                attackDecay = WithAttackBoostDecay;
            }
            else
            {
                attackDecay = AgainstAttackBoostDecay;
            }
        }

        _attackBoostVelocity.x = Mathf.MoveTowards(_attackBoostVelocity.x, 0f, attackDecay * Time.fixedDeltaTime);
    }
    private void HandleSlide()
    {
    
        if (_isSliding)
        {

            bool slideHeld = _slide.action.IsPressed();
            bool grounded = _groundCheck.IsGrounded;
            bool blocked = IsPlayerHitboxBlocked();
           


            if ((!slideHeld || !grounded) && !blocked)
            {
                _isSliding = false;
                _slideVelocity = Vector2.zero;

                _playerHitbox.SetActive(true);
                _slideHitbox.SetActive(false);
              

                return;
            }

            _slideVelocity.x = _slideDirection * MoveSpeed * SlideSpeedMultiplier;
            _slideVelocity.y = 0f;

            return;
        }

        _slideVelocity = Vector2.zero;

        _playerHitbox.SetActive(true);
        _slideHitbox.SetActive(false);

        if (!_abilities.CanSlide) return;
        if (!_groundCheck.IsGrounded) return;
        if (!_slide.action.IsPressed()) return;
        if (_isDashing || _isAttacking) return;

        if (_moveDirection != 0) _slideDirection = Mathf.Sign(_moveDirection);
        else _slideDirection = _facingDirection;

        _isSliding = true;

        if (_isSliding == true)
            {
            Debug.Log("Sliding");
        }
        _playerHitbox.SetActive(false);
           _slideHitbox.SetActive(true);

    }
    private void HandleVelocityCalculation()
    {
        //and other external forces
        _rb.linearVelocity = _walkVelocity + _dashVelocity + _slideVelocity + _dashBoostVelocity + _attackBoostVelocity + _jumpVelocity;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (_isSliding || !_abilities.CanJump) return;

        _jumpBufferTimer = JumpBufferTime;

        if (_isDashing)
        {
            _dashJumpBuffered = true;
        }
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if (_isSliding || _isDashing || !_abilities.CanDash || _dashUsed || _dashCooldownTimer > 0) return;

        Vector2 direction = _move.action.ReadValue<Vector2>();

        if (direction == Vector2.zero) direction = new Vector2(transform.localScale.x, 0f);

        audioSource.PlayOneShot(dashSound, 0.3f);//LUKAS
        _dashDirection = direction.normalized;
        _dashTimer = DashDuration;
        _isDashing = true;
        _dashUsed = true;
        _dashCooldownTimer = DashCooldown;
    }
    private void Attack(InputAction.CallbackContext context)
    {
        if (!_abilities.CanAttack || _isDashing || _isAttacking || _attackCooldownTimer > 0) return;

        _attackDirection = _facingDirection;
        audioSource.PlayOneShot(attackSound, 0.3f);
        _attackHitbox.SetActive(true);
        _isAttacking = true;
        _attackTimer = AttackDuration;
    }

    private bool IsPlayerHitboxBlocked()
    {
        Collider2D collider = _playerHitbox.GetComponent<Collider2D>();

        if (collider == null) return false;

        Bounds bounds = collider.bounds;

        return Physics2D.OverlapBox(bounds.center, bounds.size, 0f, _solidLayers) != null;
    }

    private void CheckWall()
    {
        Collider2D collider = _playerHitbox.GetComponent<Collider2D>();

        if (collider == null)
        {
            _isTouchingWall = false;
            _wallDirection = 0f;
            return;
        }

        Bounds bounds = collider.bounds;

        float checkDistance = 0.05f;

        RaycastHit2D rightWall = Physics2D.Raycast(new Vector2(bounds.max.x, bounds.center.y), Vector2.right, checkDistance, _solidLayers);

        RaycastHit2D leftWall = Physics2D.Raycast(new Vector2(bounds.min.x, bounds.center.y), Vector2.left, checkDistance, _solidLayers);

        if (rightWall.collider != null)
        {
            _isTouchingWall = true;
            _wallDirection = 1f;
        }
        else if (leftWall.collider != null)
        {
            _isTouchingWall = true;
            _wallDirection = -1f;
        }
        else
        {
            _isTouchingWall = false;
            _wallDirection = 0f;
        }
    }

    public void Respawn()
    {
        transform.position = RespawnPoint.position;
        _walkVelocity = Vector2.zero;
        _dashVelocity = Vector2.zero;
        _jumpVelocity = Vector2.zero;
        _slideVelocity = Vector2.zero;
        _dashBoostVelocity = Vector2.zero;
        _attackBoostVelocity = Vector2.zero;
    }
}