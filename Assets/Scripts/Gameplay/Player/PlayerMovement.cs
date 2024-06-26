using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private struct PlayerInputs
    {
        public Vector2 Direction;
        public Vector2 MouseDelta;
        public bool Jump;
        public bool Sprint;
    }

    [Header("Required References")]
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private Transform _thirdPersonCamera;
    [SerializeField] private CinemachineVirtualCamera _cinemachineCamera;
    [SerializeField] private Animator _animator;

    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField] private float _runSpeed = 3f;
    [SerializeField] private float _jumpHeight = 3f;
    [SerializeField] private float _hitJumpHeight = 6f;
    [SerializeField] private float _rotationSmoothTime = 5;
    [SerializeField] private float _smoothVelocitySpeed = 5f;

    [Header("Gravity Settings")]
    [SerializeField] private float _gravity = -9.18f;
    [SerializeField] private float _terminalVelocity = 20f;

    [Header("Ground Settings")]
    [SerializeField] private float _feetOffset = -1f;
    [SerializeField] private float _groundCheckRadius = 0.3f;
    [SerializeField] private float _groundRayLength = 0.3f;
    [SerializeField] private LayerMask _groundLayerMask;

    [Header("Camera Settings")]
    [SerializeField] private float _cameraYMin = -30;
    [SerializeField] private float _cameraYMax = 70;
    [SerializeField] private bool _cameraInvertY;

    private CharacterController _characterController;
    private Oscillator _currentMovingGround;
    private PlayerInputs _inputs;

    private bool _isGrounded;
    private bool _wasGroundedLastFrame;

    private float _currentSpeed;
    private float _verticalVelocity;

    private float _cameraYaw;
    private float _cameraPitch;
    private float _targetRotation;
    private float _rotationVelocity;

    private Vector3 _spawnPosition;

    private float _animationBlend;

    private int _animIdJump = Animator.StringToHash("Jump");
    private int _animIdSpeed = Animator.StringToHash("Speed");
    private int _animIdFall = Animator.StringToHash("Fall");
    private int _animIdHit = Animator.StringToHash("Hit");


    private void Start()
    {
        CollectComponents();
    }

    private void Update()
    {
        CollectInputs();
        CheckGround();

        JumpAndGravity();
        Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    public void PushFromDamage()
    {
        _verticalVelocity = Mathf.Sqrt(_hitJumpHeight * -2f * _gravity);
        _isGrounded = false;
        _animator.SetTrigger(_animIdHit);
    }

    public void WarpToRespawnLocation()
    {
        _characterController.enabled = false;
        Vector3 delta = _spawnPosition - transform.position;
        transform.position = _spawnPosition;
        _characterController.enabled = true;

        _cinemachineCamera.OnTargetObjectWarped(_cinemachineCamera.Follow, delta);
        _currentSpeed = 0;
    }

    private void CollectComponents()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void CollectInputs()
    {
        _inputs = new()
        {
            Direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            Jump = Input.GetButtonDown("Jump"),
            Sprint = Input.GetButton("Sprint"),
            MouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")),
        };
    }

    private void CheckGround()
    {
        _currentMovingGround = null;
        _wasGroundedLastFrame = _isGrounded;

        Vector3 origin = transform.position + Vector3.up * _feetOffset;
        _isGrounded = Physics.SphereCast(origin, _groundCheckRadius, Vector3.down, out RaycastHit hitInfo, _groundRayLength, _groundLayerMask, QueryTriggerInteraction.Ignore);

        if (_isGrounded) { _currentMovingGround = hitInfo.transform.GetComponent<Oscillator>(); }

        Debug.DrawRay(origin, Vector3.down * (_groundRayLength), Color.red);

    }

    private void Move()
    {
        float targetSpeed = _inputs.Sprint ? _runSpeed : _walkSpeed;

        if (_inputs.Direction == Vector2.zero) targetSpeed = 0.0f;

        const float SPEED_OFFSET = 0.1f;

        // accelerate or decelerate to target speed
        if (_currentSpeed < targetSpeed - SPEED_OFFSET ||
            _currentSpeed > targetSpeed + SPEED_OFFSET)
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed,
                Time.deltaTime * _smoothVelocitySpeed);
        }
        else
        {
            _currentSpeed = targetSpeed;
        }

        const float ANIMATION_BLEND_ZERO_THRESHOLD = 0.01f;
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _smoothVelocitySpeed);
        if (_animationBlend < ANIMATION_BLEND_ZERO_THRESHOLD) { _animationBlend = 0f; }

        Vector3 inputDirection = new Vector3(_inputs.Direction.x, 0.0f, _inputs.Direction.y).normalized;

        if (_inputs.Direction != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _thirdPersonCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                _rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        Vector3 movingGroundVelocity = _currentMovingGround ? _currentMovingGround.GetVelocity() : Vector3.zero;

        _characterController.Move(targetDirection.normalized * (_currentSpeed * Time.deltaTime) +
                  Vector3.up * _verticalVelocity * Time.deltaTime + movingGroundVelocity);


        _animator.SetFloat(_animIdSpeed, _animationBlend);
    }

    private void JumpAndGravity()
    {
        if (_isGrounded)
        {
            if (!_wasGroundedLastFrame && !_currentMovingGround)
            {
                _spawnPosition = transform.position;
            }

            _animator.SetBool(_animIdFall, false);
            // Stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                const float STICK_TO_GROUND_FORCE = -2f;
                _verticalVelocity = STICK_TO_GROUND_FORCE;
            }

            if (_inputs.Jump)
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                _animator.SetTrigger(_animIdJump);
            }
        }
        else
        {
            _animator.SetBool(_animIdFall, true);
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }
    }

    private void CameraRotation()
    {
        const float MOUSE_DELTA_THRESHOLD = 0.01f;
        if (_inputs.MouseDelta.magnitude >= MOUSE_DELTA_THRESHOLD)
        {
            float cameraInvertY = _cameraInvertY ? -1 : 1;
            _cameraYaw += _inputs.MouseDelta.x;
            _cameraPitch += _inputs.MouseDelta.y * cameraInvertY;
        }

        // Also clamp yaw so values stay within 360 degrees
        _cameraYaw = ClampAngle(_cameraYaw, float.MinValue, float.MaxValue);
        _cameraPitch = ClampAngle(_cameraPitch, _cameraYMin, _cameraYMax);

        // Cinemachine will follow this target
        _cameraTarget.rotation = Quaternion.Euler(_cameraPitch,
            _cameraYaw, 0.0f);
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}
