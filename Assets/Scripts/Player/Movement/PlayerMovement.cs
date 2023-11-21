using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ForceBody forceBody;
    [SerializeField] private AnimationManager animationManager;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 1.5f;
    [SerializeField, Range(0,1)] private float groundCheckThreshold;

    [Header("Forces")]
    [SerializeField] private Force jumpForce;
    [SerializeField] private Force movementForce;

    [Header("Deadzone")] [SerializeField] private float deadzone = 0.41f;

    [Header("Event Variables")]
    [SerializeField] private UnityEvent onJumpStart = new UnityEvent();
    [SerializeField] private UnityEvent onJump = new UnityEvent();

    [SerializeField] private UnityEvent onJumpPeak = new UnityEvent();
    [SerializeField] private UnityEvent onPreJumpPeak = new UnityEvent();

    [SerializeField] private UnityEvent<bool> onGroundStateChange = new UnityEvent<bool>();
    [SerializeField] private UnityEvent onGroundExit = new UnityEvent();
    [SerializeField] private UnityEvent onGroundLand = new UnityEvent();
    [SerializeField] private UnityEvent onAir = new UnityEvent();

    [SerializeField] private UnityEvent onStartMoving = new UnityEvent();
    [SerializeField] private UnityEvent onGroundWalk = new UnityEvent();
    [SerializeField] private UnityEvent onStopMoving = new UnityEvent();

    [SerializeField] private UnityEvent onTurnRight = new UnityEvent();
    [SerializeField] private UnityEvent onTurnLeft = new UnityEvent();

    [SerializeField] private UnityEvent onTurnLeftGrounded = new UnityEvent();
    [SerializeField] private UnityEvent onTurnRightGrounded = new UnityEvent();

    private Vector2 _moveDirection;
    private Vector3 _lastVelocity;
    private Vector3 _velocity;

    private float _currentAirSpeed;
    private float _currentMoveDuration;
    private float _currentMaxAirAcceleration;
    private float _responsiveMaxAirAcceleration = 0.1f;
    private float _staticMaxAirAcceleration = 0.03f;

    private bool _isGrounded;
    private bool _canJumpInAir;
    private bool _jumpPrimed;
    private bool _shouldJump;
    private bool _groundedAtJumpStart = false;
    private bool _isCounting;
    private int _jumpDelay = 5;// frames
    private int _jumpDelayCounter = 0;
    private bool _isFalling;
    private bool _isJumpForceActive;
    private bool _isMovementDisabled = false;
    private bool _isMoving;
    private bool _isExhausted;
    private bool _isReactive = true;
    private bool _isSprinting;
    private bool _checkForGround;

    private static readonly int Velocity = Animator.StringToHash("Velocity");
    private static readonly int isGrounded = Animator.StringToHash("isGrounded");
    private static readonly int IsFalling = Animator.StringToHash("isFalling");
    private static readonly int isSprinting = Animator.StringToHash("isSprinting");
    private static readonly int Exhausted = Animator.StringToHash("isExhausted");
    private static readonly int InputX = Animator.StringToHash("InputX");

    [Header("Jump Variables")]
    [SerializeField] private float preJumpPeakOffset = 0.3f;

    [Header("Deceleration Variables")]
    [SerializeField] private float currentMoveDuration;
    [SerializeField] private float decelarationMultiplier = 5;

    [SerializeField] private List<MovementSettings> settings;
    private readonly Dictionary<MovementStates, MovementSettings> _movementSettings = new Dictionary<MovementStates, MovementSettings>();
    public MovementStates currentMovementState = MovementStates.Walking;

    // todo: SRP van deze class evalueren

    private void Awake()
    {
        for (int i = 0; i < settings.Count; i++)
        {
            var setting = settings[i];
            _movementSettings.Add(setting.id, setting);
        }
        
        forceBody.onCollisionEvent.AddListener(OnCollide);
    }

    private void FixedUpdate()
    {
        if (_isMovementDisabled) return;
        
        CheckGroundState();
        
        if (_shouldJump)
        {
            _jumpDelayCounter++;
            if (_jumpDelayCounter == _jumpDelay)
            {
                _jumpDelayCounter = 0;
                _shouldJump = false;
                Jump();
            }
        }

        if (MayJump()) StartJump();
        else _jumpPrimed = false;

        if (_isMoving) currentMoveDuration += Time.deltaTime;

        CalculateVelocity();
        UpdateAnimation(Mathf.Abs(_velocity.x));
        
        if (_isGrounded && _velocity.magnitude == 0 && _lastVelocity.magnitude > 0) onStopMoving?.Invoke();

        if (_isGrounded && _velocity.magnitude > 0 && _lastVelocity.magnitude == 0) onStartMoving?.Invoke();

        if (_isGrounded && _velocity.magnitude > 0) onGroundWalk?.Invoke();

        if (_isGrounded)
        {
            IsJumping = false;
            transform.rotation = Quaternion.Euler(0,0,0);
        }

        _lastVelocity = _velocity;
        if (_velocity.x == 0)
        {
            ResetMoveDuration();
            return;
        }
        UpdateMovement(_velocity);
    }

    private Vector3 CalculateVelocity()
    {
        var maxAcceleration = _isGrounded ? CurrentMoveSetting.maxGroundAcceleration : _currentMaxAirAcceleration;
        var speed = _isGrounded ? CurrentMoveSetting.groundSpeed : _currentAirSpeed;
        var desiredMove = _moveDirection * speed;
        _velocity = new Vector3(desiredMove.x, 0, 0);
        var acceleration = _velocity - _lastVelocity;

        if (Mathf.Abs(acceleration.x) > maxAcceleration)
        {
            _velocity.x = _lastVelocity.x + acceleration.x / Mathf.Abs(acceleration.x) * maxAcceleration;
        }

        _currentMaxAirAcceleration = _moveDirection.x != 0 ? _responsiveMaxAirAcceleration : _staticMaxAirAcceleration;
        if (IsJumping || _isJumpForceActive) UpdateAirSettings(desiredMove);
        return _velocity;
    }

    private void UpdateAnimation(float speed)
    {
        animationManager.SetBool(isGrounded, _isGrounded);
        animationManager.SetBool(IsFalling, _isFalling);
        animationManager.SetBool(isSprinting, IsSprinting);
        animationManager.SetFloat(Velocity, Math.Abs(speed));
        animationManager.SetBool(Exhausted,_isExhausted);
    }

    private void UpdateMovement(Vector3 velocity)
    {
        movementForce.Direction = velocity;
        forceBody.Add(movementForce);
    }

    private void UpdateAirSettings(Vector2 desiredMove)
    {
        var currentSpeed = desiredMove.x / _moveDirection.x;
        _currentAirSpeed = Mathf.Max(currentSpeed, CurrentMoveSetting.airSpeed);
    }

    private bool MayJump()
    {
        return _jumpPrimed && CanJump && !_isJumpForceActive && forceBody.IsEnabled && CanUseMove;
    }

    private void StartJump()
    {
        ResetMoveDuration();

        onJumpStart?.Invoke();

        _shouldJump = true;
        _jumpPrimed = false;
        _groundedAtJumpStart = CanJump;
    }

    private void CheckGroundState()
    {
        if (!_checkForGround) return;
        var hitsGround = forceBody.CheckFrontalCollision(Vector3.down, groundCheckDistance, groundCheckThreshold);
        if (_isGrounded == hitsGround) return;
        SetGroundState(hitsGround);
    }

    private void SetGroundState(bool hitsGround)
    {
        if (hitsGround && !_isGrounded)
        {
            onGroundLand?.Invoke();
            ResetMoveDuration();
        }
        if (!hitsGround && _isGrounded)
        {
            onGroundExit?.Invoke();
            onAir?.Invoke();
        }
        onGroundStateChange?.Invoke(hitsGround);

        _isGrounded = hitsGround;

        if (hitsGround) _isFalling = false;
    }

    private void Jump()
    {
        if (!isActiveAndEnabled) return;
        if (!_groundedAtJumpStart) return;

        Invoke("PreJumpPeak", jumpForce.Duration - preJumpPeakOffset);
        
        IsJumping = true;
        _isJumpForceActive = true;
        _canJumpInAir = false;
        _isFalling = false;
        SetGroundState(false);
        _checkForGround = false;
        onJump?.Invoke();

        forceBody.Add(jumpForce, new CallbackConfig()
        {
            OnFinish = JumpPeak,
            OnCancel = JumpPeak
        });
    }

    public bool JumpPrimer(InputAction.CallbackContext context)
    {
        if (!CanJump) return false;
        return _jumpPrimed = true;
    }

    public void Move(Vector2 dir)
    {
        animationManager.SetFloat(InputX, Math.Abs(dir.x));
        if (dir.x > deadzone) dir.x = 1;
        else if (dir.x < -deadzone) dir.x = -1;
        else dir.x = 0;

        if (!isActiveAndEnabled) return;

        _moveDirection = dir;

        if (!isActiveAndEnabled) return;

        CheckMoveDirection(dir);
        RotatePlayer(dir);
        _isMoving = dir.x != 0;
    }

    public void StopMovement()
    {
        Move(Vector2.zero);
        _isMoving = false;
    }

    public void RotatePlayer(Vector2 dir)
    {
        Vector3 localScale = transform.localScale;
        if (dir.x == 0) return;

        transform.localScale = dir.x > 0
            ? transform.localScale = new Vector3(Math.Abs(localScale.x), localScale.y, localScale.z)
            : transform.localScale = new Vector3(-Math.Abs(localScale.x), localScale.y, localScale.z);
    }

    private void JumpPeak()
    {
        _isFalling = true;
        _isJumpForceActive = false;
        onJumpPeak?.Invoke();
    }

    private void PreJumpPeak()
    {
        onPreJumpPeak?.Invoke();
    }

    private void CheckMoveDirection(Vector2 newDirection)
    {
        if (newDirection.x == 0) return;
        if (newDirection.x < 0 && _lastVelocity.x >= 0)
        {
            onTurnLeft?.Invoke();
            if (!_isGrounded) return;
            onTurnLeftGrounded?.Invoke();
        }
        else if (newDirection.x > 0 && _lastVelocity.x <= 0)
        {
            onTurnRight?.Invoke();
            if (!_isGrounded) return;
            onTurnRightGrounded?.Invoke();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.collider) gameObject.transform.rotation = Quaternion.Euler(0,0,0);
    }

    public void AdjustReactionTime()
    {
        AdjustReactionTime(false);
    }
    
    public void AdjustReactionTime(bool becomesNotReactive)
    {
        if (IsSprinting || becomesNotReactive == !_isReactive) return;

        if (becomesNotReactive && _isGrounded && _isReactive)
        {
            movementForce.Duration *= decelarationMultiplier;
            _movementSettings[MovementStates.Sprinting] *= decelarationMultiplier;
            _movementSettings[MovementStates.Walking] *= decelarationMultiplier;
            _isReactive = false;
        }
        else if(!_isReactive && !becomesNotReactive)
        {
            movementForce.Duration /= decelarationMultiplier;
            _movementSettings[MovementStates.Sprinting] /= decelarationMultiplier;
            _movementSettings[MovementStates.Walking] /= decelarationMultiplier;
            _isReactive = true;
        }
    }

    private void OnCollide(CollisionForceEvent collisionForceEvent)
    {
        if (!collisionForceEvent.resetsGravity) return;
        _checkForGround = true;
    }
    
    private void ResetMoveDuration() => currentMoveDuration = 0;
    
    public bool IsJumping { get; private set; }

    public bool CanUseMove { get; set; } = true;

    public bool IsGrounded
    {
        get => _isGrounded;
        private set => _isGrounded =value;
    }

    public bool CanJump
    {
        get => _canJumpInAir || _isGrounded;
        set => _canJumpInAir = value;
    }

    public Vector3 LastVelocity
    {
        get => _lastVelocity;
        set => _lastVelocity = value;
    }

    public bool IsMoving
    {
        get => _isMoving;
        set => _isMoving = value;
    }

    public bool IsExhausted
    {
        get => _isExhausted;
        set => _isExhausted = value;
    }

    public bool IsSprinting => currentMovementState == MovementStates.Sprinting;
    public bool IsWalking => currentMovementState == MovementStates.Walking;
    private MovementSettings CurrentMoveSetting => _movementSettings[currentMovementState];
    public UnityEvent<bool> GetGroundChangeEvent => onGroundStateChange;
}