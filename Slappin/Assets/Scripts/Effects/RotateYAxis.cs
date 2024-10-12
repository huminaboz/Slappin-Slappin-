using UnityEngine;

public class RotateYAxis : MonoBehaviour
{
    public float rotationSpeed = 10f;

    void Update()
    {
        Vector3 currentRotation = transform.eulerAngles;
        float newYRotation = currentRotation.y + (rotationSpeed * Time.deltaTime);

        // Set the new rotation while maintaining the X and Z values
        transform.eulerAngles = new Vector3(currentRotation.x, newYRotation, currentRotation.z);
    }
}
