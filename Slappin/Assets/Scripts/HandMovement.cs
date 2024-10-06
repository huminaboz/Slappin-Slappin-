using UnityEngine;

public class HandMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f; // Default movement speed
    [SerializeField] private float boostedSpeedMultiplier = 2f; // How much faster when holding the right trigger

    private void Update()
    {
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
            transform.Translate(direction * (currentSpeed * Time.deltaTime), Space.World);
        }
    }
}