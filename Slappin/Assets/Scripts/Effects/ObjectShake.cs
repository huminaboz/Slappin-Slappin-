using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ObjectShake : MonoBehaviour
{
    [SerializeField] private Transform objectToShake;
    private const float ShakeIntensityDefault = 0.025f;
    private const float ShakeDurationDefault = 0.25f;

    private Vector3 _originalPosition;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        objectToShake = GetComponent<Transform>();
    }

    private void Start()
    {
        _originalPosition = objectToShake.localPosition;
    }
    
    public void StartShake(float intensity = ShakeIntensityDefault, float duration = ShakeDurationDefault)
    {
        // If there's already a shake running, stop it before starting a new one
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            objectToShake.localPosition = _originalPosition; // Reset position in case previous shake was interrupted
        }

        // Start a new shake with the given intensity and duration
        _shakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    //TODO:: Open this up to be a shakeInfinite and a ShakeDuration
    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        // float elapsedTime = 0f;

        // Keep shaking until the duration is over
        while (true)
        {
            // Apply a random shake by changing the camera position
            objectToShake.localPosition = _originalPosition + Random.insideUnitSphere * intensity;

            // elapsedTime += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }

        // RestoreDefaults();
        //
        // // Clear the coroutine reference
        // _shakeCoroutine = null;
    }

    private void RestoreDefaults()
    {
        objectToShake.localPosition = _originalPosition;
    }

    public void StopShake()
    {
        RestoreDefaults();
        StopAllCoroutines();
        _shakeCoroutine = null;
    }
}