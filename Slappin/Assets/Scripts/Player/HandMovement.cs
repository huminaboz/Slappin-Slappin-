using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class HandMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f; // Default movement speed

    // [SerializeField] private float boostedSpeedMultiplier = 2f; // How much faster when holding the right trigger
    [SerializeField] private Transform handPositioner;
    [SerializeField] private Player thisPlayer;

    [HideInInspector] public Rigidbody _rigidbody;
    [SerializeField] private Transform northEastPoint;
    [SerializeField] private Transform northWestPoint;
    [SerializeField] private Transform southEastPoint;
    [SerializeField] private Transform southWestPoint;

    private SnapToTheGround _snapToTheGround;
    private InputSystem_Actions _inputSystem;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _snapToTheGround = GetComponent<SnapToTheGround>();
        _inputSystem = new InputSystem_Actions();
        _inputSystem.Player.Enable();
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        _rigidbody.velocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (!StateGame.PlayerInGameControlsEnabled) return;

        // Read inputs from the left joystick
        // float moveX = Input.GetAxis("Horizontal");
        // float moveZ = Input.GetAxis("Vertical");
        Vector2 movement = _inputSystem.Player.Move.ReadValue<Vector2>();

        // Calculate movement direction
        Vector3 direction = new Vector3(movement.x, 0, movement.y).normalized;
        // if (moveX == 0f && moveZ == 0f) return;

        // bool isBoosting = Input.GetAxis("RTrigger") > 0f;
        bool isBoosting = _inputSystem.Player.Boost.IsPressed();
        // Apply speed boost if holding right trigger
        float currentSpeed = isBoosting ? moveSpeed * StatLiason.I.Get(Stat.MoveBoostSpeed) : moveSpeed;
        // Debug.LogWarning($"Current Speed: {currentSpeed}");

        // Apply movement to the player transform
        if (direction.magnitude >= 0.1f)
        {
            _rigidbody.velocity = direction * (currentSpeed * Time.deltaTime);
            thisPlayer.CurrentAttackType?.SetFacingDirection();
        }
        else
        {
            _rigidbody.velocity = Vector3.zero;
        }

        _snapToTheGround.StayYDistanceFromGround(_rigidbody);
        RestrainPosition(_rigidbody);
    }

    private void RestrainPosition(Rigidbody rb)
    {
        float northMinX = Mathf.Min(northWestPoint.position.x, northEastPoint.position.x);
        float northMaxX = Mathf.Max(northWestPoint.position.x, northEastPoint.position.x);
        float southMinX = Mathf.Min(southWestPoint.position.x, southEastPoint.position.x);
        float southMaxX = Mathf.Max(southWestPoint.position.x, southEastPoint.position.x);
        float minZ = Mathf.Min(southWestPoint.position.z, southEastPoint.position.z);
        float maxZ = Mathf.Max(northWestPoint.position.z, northEastPoint.position.z);

        Vector3 currentPosition = rb.position;
        Vector3 clampedPosition = currentPosition;

        bool outOfXBounds = currentPosition.x < Mathf.Lerp(southMinX, northMinX, (currentPosition.z - minZ) / (maxZ - minZ)) 
                            || currentPosition.x > Mathf.Lerp(southMaxX, northMaxX, (currentPosition.z - minZ) / (maxZ - minZ));
        bool outOfZBounds = currentPosition.z < minZ || currentPosition.z > maxZ;

        if (outOfZBounds || outOfXBounds)
        {
            clampedPosition.z = Mathf.Clamp(currentPosition.z, minZ, maxZ);

            float normalizedZ = (clampedPosition.z - minZ) / (maxZ - minZ);
            float minX = Mathf.Lerp(southMinX, northMinX, normalizedZ);
            float maxX = Mathf.Lerp(southMaxX, northMaxX, normalizedZ);

            clampedPosition.x = Mathf.Clamp(currentPosition.x, minX, maxX);

            rb.position = clampedPosition;
        }
    }
}