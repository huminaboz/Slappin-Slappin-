using UnityEngine;
using UnityEngine.Serialization;

public class WalkTowardsTransform : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float stoppingPoint = -2f;
    [SerializeField] private Transform hurtLine;

    private void Start()
    {
        if (targetTransform == null)
        {
            Debug.LogError("target transform is null");
        }
    }

    private void Update()
    {
        if (transform.localPosition.z + transform.localScale.z *.05 <= hurtLine.localPosition.z) return;
        
        // Get the direction from the current position to the target position
        Vector3 direction = (targetTransform.position - transform.position);
        direction.y = 0; // Ignore the Y axis for movement

        // Move the object towards the target
        Vector3 newPosition = transform.position + direction.normalized * walkSpeed * Time.deltaTime;
        transform.position = newPosition;

        // If there's movement, rotate to face the target
        if (direction.magnitude > 0.1f)
        {
            // Calculate the desired rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // Rotate towards the target over time
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
