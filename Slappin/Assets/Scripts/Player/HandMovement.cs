using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class HandMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f; // Default movement speed
    [SerializeField] private float boostedSpeedMultiplier = 2f; // How much faster when holding the right trigger
    [SerializeField] private Transform slapPositioner;

    private Vector3 relativePositioning;
    [HideInInspector] public Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        relativePositioning = slapPositioner.position - transform.position;
        Debug.Log($"Relative positioning between shadow and hand is: {relativePositioning}");
        SetSlapPosition();
    }

    private void SetSlapPosition()
    {
        Vector3 offsetPosition = transform.position + relativePositioning;
        //Move the slap hand with the shadow only on X and Z 
        slapPositioner.position = new Vector3(offsetPosition.x, slapPositioner.position.y, offsetPosition.z);
    }

    private void FixedUpdate()
    {
        if (!StateGame.PlayerInGameControlsEnabled) return;

        // Read inputs from the left joystick
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 direction = new Vector3(moveX, 0, moveZ).normalized;

        bool isBoosting = Input.GetAxis("RTrigger") > 0f;
        // Apply speed boost if holding right trigger
        float currentSpeed = isBoosting ? moveSpeed * boostedSpeedMultiplier : moveSpeed;

        // Apply movement to the player transform
        if (direction.magnitude >= 0.1f)
        {
            _rigidbody.velocity = direction * (currentSpeed * Time.deltaTime);

            SetSlapPosition();
        }
        else
        {
            _rigidbody.velocity = Vector3.zero;
        }
    }
}