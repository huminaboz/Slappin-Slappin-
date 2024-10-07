using UnityEngine;

public class WalkTowardsTransform : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5f;

    //TODO:: Can set this up to target a random X position at the hurt line

    private void Start()
    {
        if (EnemyTarget.I.targetTransform == null)
        {
            Debug.LogError("target transform is null");
        }
    }

    private void Update()
    {
        if (transform.localPosition.z + transform.localScale.z *.05 <= EnemyTarget.I.hurtLine.localPosition.z) return;
        
        // Get the direction from the current position to the target position
        Vector3 direction = (EnemyTarget.I.targetTransform.position - transform.position);
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
