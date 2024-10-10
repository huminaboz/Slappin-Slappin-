using System.Collections;
using UnityEngine;

public class ArcToCamera : MonoBehaviour
{
    [SerializeField] private float flightDuration = 2f; // Time to complete the arc
    [SerializeField] private float arcHeight = 5f; // Height of the arc

    // private TrailRenderer trailRenderer;
    private float flightProgress = 0f;
    private Vector3 startPosition;
    private Vector3 cameraPosition;

    private Vector3 controlPoint; // Point to define the arc
    private float startTime;

    private Pickup pickup;

    private void Awake()
    {
        // trailRenderer = GetComponent<TrailRenderer>();

        pickup = GetComponent<Pickup>();
    }

    private void OnEnable()
    {
        // Set initial state to ensure trail is not visible at the start
        // trailRenderer.enabled = false;
        startPosition = transform.position;
    }

    public void FlyTowardsCamera()
    {
        cameraPosition = Camera.main.transform.position;
        controlPoint = (startPosition + cameraPosition) / 2 + Vector3.up * arcHeight;

        flightProgress = 0f;
        startTime = Time.time;

        // Enable the trail renderer
        // trailRenderer.enabled = true;

        StartCoroutine(Fly());
    }

    private IEnumerator Fly()
    {
        flightProgress = (Time.time - startTime) / flightDuration;

        while (flightProgress < 1f)
        {
            // Calculate current position based on a quadratic Bezier curve for the arc
            Vector3 newPos = CalculateArcPosition(startPosition, controlPoint, cameraPosition, flightProgress);
            transform.position = newPos;
            yield return null;
        }

        pickup.ReturnObjectToPool();
    }


    Vector3 CalculateArcPosition(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        // Quadratic Bezier curve equation
        return (1 - t) * (1 - t) * start + 2 * (1 - t) * t * control + t * t * end;
    }
}