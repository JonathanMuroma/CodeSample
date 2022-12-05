using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    Transform tr;
    Transform cameraTransform;

    PlayerAnimationHandler playerAnimationHandler;

    private Vector3 hitPosition;
    private Vector3 hitNormal;
    private float hitDistance;

    Vector3 currentGroundAdjustmentVelocity = Vector3.zero;

    bool isGrounded = false;
    bool jumped = false;

    Vector3 grav = Vector3.zero;
    Vector3 velocity = Vector3.zero;
    Vector3 jumpForce = Vector3.zero;
    float currentJumpStartTime = 0f;
    public float gravity = 10f;
    public float movementSpeed = 7f;
    public float groundFriction = 100f;

    bool IsUsingExtendedSensorRange = true;
    float baseSensorRange = 0f;
    Vector3 playerVelocity;

    float colliderHeight;
    [Range(0, 1), SerializeField] float stepHeightRatio = 0.25f;

    private List<Collider> hitColliders = new List<Collider>();
    private List<Transform> hitTransforms = new List<Transform>();
    private Vector3 backupNormal;


    public BoolVariable inputAllowed = null;
    public BoolVariable jumpUnlocked = null;


    [SerializeField]
    private AudioSource jumpSound = null;
    [SerializeField]
    private AudioSource landSound = null;

    bool prevGroundedStatus = false;

    PlayerControls controls;
    Vector2 move;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.Jump.performed += ctx => JumpInput();

        controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => move = Vector2.zero;

        controls.Gameplay.Camera.ApplyBindingOverride(new InputBinding { overrideProcessors = "invertVector2(invertX=true,invertY=true)" });
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();

    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        colliderHeight = GetComponent<CapsuleCollider>().height;
        cameraTransform = Camera.main.transform;

        jumpForce = tr.up * 10f;

        playerAnimationHandler = transform.GetChild(1).GetComponent<PlayerAnimationHandler>();
        if (playerAnimationHandler == null)
            Debug.LogWarning("PlayerHandler is missing!");

    }

    void Update()
    {
        if(inputAllowed.value)
        {
            controls.Gameplay.Enable();
        } else
        {
            controls.Gameplay.Disable();
        }

        if (move.x != 0f || move.y != 0f)
        {
            playerAnimationHandler.SetWalkAnimation(true);
        }
        else
        {
            playerAnimationHandler.SetWalkAnimation(false);
        }
    }

    void JumpInput()
    {
        if (isGrounded && jumpUnlocked.value)
        {
            jumped = true;  
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        CheckGround();

        PlayLandedSound();

        HandleMomentum();

        JumpCheck();

        IsUsingExtendedSensorRange = isGrounded;

        Vector3 _inputVelocity = CalculateMovementDirection();

        playerVelocity = velocity + _inputVelocity;

        AddVelocity(playerVelocity);


    }


    void CheckGround()
    {
        currentGroundAdjustmentVelocity = Vector3.zero;
        RaycastHit _hit;
        float _Sensorlength = CalculateSensorRange();

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out _hit, _Sensorlength))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * _Sensorlength, Color.yellow);

            hitPosition = _hit.point;
            hitNormal = _hit.normal;

            hitDistance = _hit.distance;
            isGrounded = true;
            playerAnimationHandler.SetJumpAnimation(false);
            playerAnimationHandler.SetFallAnimation(false);

            float _upperLimit = ((colliderHeight * tr.localScale.x) * (1f - stepHeightRatio)) * 0.5f;
            float _middle = _upperLimit + (colliderHeight * tr.localScale.x) * stepHeightRatio;
            float _distanceToGo = _middle - hitDistance;

            //Set new ground adjustment velocity for the next frame;
            currentGroundAdjustmentVelocity = tr.up * (_distanceToGo / Time.fixedDeltaTime);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * _Sensorlength, Color.red);

            isGrounded = false;
            if(!jumped)
            {
                playerAnimationHandler.SetFallAnimation(true);
            }
        }

    }

    void HandleMomentum()
    {
        Vector3 _verticalMomentum = Vector3.zero;
        Vector3 _horizontalMomentum = Vector3.zero;

        if (velocity != Vector3.zero)
        {
            _verticalMomentum = ExtractDotVector(velocity, tr.up);
            _horizontalMomentum = velocity - _verticalMomentum;
        }

        _verticalMomentum -= tr.up * gravity * Time.deltaTime;

        if (isGrounded)
        {
            _verticalMomentum = Vector3.zero;
        }

        if (isGrounded)
            _horizontalMomentum = IncrementVectorTowardTargetVector(_horizontalMomentum, groundFriction, Time.deltaTime, Vector3.zero);

        velocity = _horizontalMomentum + _verticalMomentum;
    }

    void JumpCheck()
    {
        if (jumped && isGrounded)
        {
            jumpSound.Play();

            currentJumpStartTime = Time.time;
            velocity += jumpForce;
            jumped = false;
            isGrounded = false;
            playerAnimationHandler.SetJumpAnimation(true);
        }
    }

    void AddVelocity(Vector3 _velocity)
    {
        rb.velocity = _velocity + currentGroundAdjustmentVelocity;
    }

    public Vector3 ExtractDotVector(Vector3 _vector, Vector3 _direction)
    {
        //Normalize vector if necessary;
        if (_direction.sqrMagnitude != 1)
            _direction.Normalize();

        float _amount = Vector3.Dot(_vector, _direction);

        return _direction * _amount;
    }

    Vector3 IncrementVectorTowardTargetVector(Vector3 _currentVector, float _speed, float _deltaTime, Vector3 _targetVector)
    {
        return Vector3.MoveTowards(_currentVector, _targetVector, _speed * _deltaTime);
    }

    Vector3 CalculateMovementDirection()
    {

        Vector3 _velocity = Vector3.zero;

        _velocity += Vector3.ProjectOnPlane(cameraTransform.right, tr.up).normalized * move.x;
        _velocity += Vector3.ProjectOnPlane(cameraTransform.forward, tr.up).normalized * move.y;


        //If necessary, clamp movement vector to magnitude of 1f;
        if (_velocity.magnitude > 1f)
            _velocity.Normalize();

        _velocity *= movementSpeed;

        return _velocity;
    }

    float CalculateSensorRange()
    {
        float sensorLenght = 0f;

        float _length = 0f;

        float _safetyDistanceFactor = 0.001f;

        _length += (colliderHeight * (1f - stepHeightRatio)) * 0.5f;
        _length += colliderHeight * stepHeightRatio;
        baseSensorRange = _length * (1f + _safetyDistanceFactor) * tr.localScale.x;

        if (IsUsingExtendedSensorRange)
            sensorLenght = baseSensorRange + (colliderHeight * tr.localScale.x) * stepHeightRatio;
        else
            sensorLenght = baseSensorRange;

        return sensorLenght;
    }

    public Vector3 GetVelocity() => playerVelocity;

    private void ResetFlags()
    {

        hitPosition = Vector3.zero;

        hitDistance = 0f;

        if (hitColliders.Count > 0)
            hitColliders.Clear();
        if (hitTransforms.Count > 0)
            hitTransforms.Clear();
    }


    private void PlayLandedSound()
    {
        

        if(isGrounded && (isGrounded != prevGroundedStatus))
        {
            landSound.Play();
        }

        prevGroundedStatus = isGrounded;
    }
}
