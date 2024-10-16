using UnityEngine;

public class Hover : MonoBehaviour
{
    public float hoverHeight = 0.5f;   // Maximum height of the hover above the starting position
    public float hoverSpeed = 2f;      // Speed of the hover (how fast it moves up and down)
    public float shiftAmount = 0.2f;     // Maximum shift on the X and Z axes
    public float shiftSpeed = 1f;
    
    private Vector3 originalPosition;  // To store the original position of the sphere

    private void OnEnable()
    {
        float heightVariance = Random.Range(-hoverHeight*.5f, hoverHeight*.5f);
        hoverHeight += heightVariance;
        
        float speedVariance = Random.Range(-hoverSpeed*.5f, hoverSpeed*.5f);
        hoverSpeed += speedVariance;
        
        float shiftAmountVariance = Random.Range(-shiftAmount*.5f, shiftAmount*.5f);
        shiftAmount += shiftAmountVariance;
        
        float shiftSpeedVariance = Random.Range(-shiftSpeed*.5f, shiftSpeed*.5f);
        shiftSpeed += shiftSpeedVariance;
    }

    public void SetOriginPosition()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new Y position using a sine wave for hover effect
        float newY = originalPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;

        // Calculate new X and Z positions for subtle horizontal shifting
        float newX = originalPosition.x + Mathf.Cos(Time.time * shiftSpeed) * shiftAmount;
        float newZ = originalPosition.z + Mathf.Sin(Time.time * shiftSpeed) * shiftAmount;

        // Apply the new position to the sphere
        transform.position = new Vector3(newX, newY, newZ);
    }
}
