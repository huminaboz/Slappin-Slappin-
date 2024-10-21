using UnityEngine;

public class SnapToTheGround : MonoBehaviour
{
    public LayerMask groundLayer;           // The ground layer to detect
    public float rayDistance = 1.5f;        // How far the ray will check downwards
    public float groundOffset = 0f;         // Default offset above the ground after snapping (0 by default)
    public float repositionSpeed = 10f;      // The speed at which the object moves to the ground (renamed)
    public bool showRaycastGizmo = true;    // Show the debug ray in the scene view
    public float forwardRayOffset = 0.5f;   // Offset to cast ray in front of object for slope detection
    public float slopeDetectionDistance = .001f; // Distance to cast the ray ahead of the object for slope detection

    void Update()
    {
        // Check the slope ahead of the object
        RaycastHit hit;
        bool hitGround = false;
        Vector3 targetPosition = transform.position;

        // Raycast downward ahead of the object to detect slopes, using slopeDetectionDistance
        Vector3 forwardRayStart = transform.position + transform.forward * slopeDetectionDistance;
        Ray rayForwardDown = new Ray(forwardRayStart + Vector3.up * forwardRayOffset, Vector3.down);

        // Check for the slope or ground in front of the object
        if (Physics.Raycast(rayForwardDown, out hit, rayDistance, groundLayer))
        {
            // Set the target Y position based on the slope or ground hit point
            targetPosition.y = hit.point.y + groundOffset;
            hitGround = true;
        }

        // If no hit in front, raycast directly below the object
        if (!hitGround)
        {
            Ray rayDown = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(rayDown, out hit, rayDistance, groundLayer))
            {
                // Set the target Y position based on the hit point below the object
                targetPosition.y = hit.point.y + groundOffset;
                hitGround = true;
            }
        }

        // If a hit is detected, smoothly move towards the target Y position
        if (hitGround)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, targetPosition.y, repositionSpeed * Time.deltaTime), transform.position.z);
        }
    }

    // Optional: Draw the debug raycast in the Scene view
    private void OnDrawGizmos()
    {
        if (showRaycastGizmo)
        {
            Gizmos.color = Color.blue;

            // Draw the forward ray downwards for slope detection
            Vector3 forwardRayStart = transform.position + transform.forward * slopeDetectionDistance;
            Gizmos.DrawLine(forwardRayStart + Vector3.up * forwardRayOffset, forwardRayStart + Vector3.up * forwardRayOffset + Vector3.down * rayDistance);

            // Draw the downward ray from the object
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayDistance);

            // Draw a sphere at the hit points
            Ray rayForwardDown = new Ray(forwardRayStart + Vector3.up * forwardRayOffset, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(rayForwardDown, out hit, rayDistance, groundLayer))
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(hit.point, 0.01f);  // Set sphere radius to 0.01f
            }

            Ray rayDown = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(rayDown, out hit, rayDistance, groundLayer))
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(hit.point, 0.01f);  // Set sphere radius to 0.01f
            }
        }
    }
}
