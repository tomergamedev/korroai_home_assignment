using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    struct PlayerInputs
    {
        public Vector2 Direction;
        public bool Jump;
        public bool Sprint;

        public Vector2 MouseDelta;
    }

    [Header("Required References")]
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private Transform _thirdPersonCamera;
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
    [SerializeField] private LayerMask _groundLayerMask;

    private CharacterController _characterController;
    private PlayerInputs _inputs;

    private bool _isGrounded;
    [SerializeField] private float _currentSpeed;
    private float _verticalVelocity;

    private float _cameraYaw;
    private float _cameraPitch;
    private float _targetRotation;
    private float _rotationVelocity;

    private float _animationBlend;
  

    // Start is called before the first frame update
    private void Start()
    {
        CollectComponents();
    }

    // Update is called once per frame
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
         _isGrounded = Physics.CheckSphere(transform.position + Vector3.up * _feetOffset, _groundCheckRadius, _groundLayerMask);
       // _isGrounded = _characterController.isGrounded;
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _inputs.Sprint ? _runSpeed : _walkSpeed;

        if (_inputs.Direction == Vector2.zero) targetSpeed = 0.0f;

        float currentMoveSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;

        const float SPEED_OFFSET = 0.1f;

        // accelerate or decelerate to target speed
        if (currentMoveSpeed < targetSpeed - SPEED_OFFSET ||
            currentMoveSpeed > targetSpeed + SPEED_OFFSET)
        {
            _currentSpeed = Mathf.Lerp(currentMoveSpeed, targetSpeed,
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

        _characterController.Move(targetDirection.normalized * (_currentSpeed * Time.deltaTime) +
                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);


        _animator.SetFloat("Speed", _animationBlend);
    }

    private void JumpAndGravity()
    {
        if (_isGrounded)
        {
            _animator.SetBool("Fall", false);
            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                const float STICK_TO_GROUND_FORCE = -2f;
                _verticalVelocity = STICK_TO_GROUND_FORCE;
            }

            // Jump
            if (_inputs.Jump)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                _animator.SetTrigger("Jump");
            }
        }
        else
        {
            _animator.SetBool("Fall", true);
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }
    }

    private void CameraRotation()
    {
        const float MOUSE_DELTA_THRESHOLD = 0.01f;
        // if there is an input and camera position is not fixed
        if (_inputs.MouseDelta.magnitude >= MOUSE_DELTA_THRESHOLD)
        {
            _cameraYaw += _inputs.MouseDelta.x;
            _cameraPitch += _inputs.MouseDelta.y;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cameraYaw = ClampAngle(_cameraYaw, float.MinValue, float.MaxValue);
        _cameraPitch = ClampAngle(_cameraPitch, -30, 70);

        // Cinemachine will follow this target
        _cameraTarget.rotation = Quaternion.Euler(_cameraPitch,
            _cameraYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + Vector3.up * _feetOffset, _groundCheckRadius);
    }
}
