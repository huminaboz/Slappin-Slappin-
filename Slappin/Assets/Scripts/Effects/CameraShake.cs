using System;
using System.Collections;
using QFSW.QC;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : Singleton<CameraShake>
{
    [Header("Run this in Quantum with 'DebugShake'")]
    [SerializeField] private float testShakeIntensity = .1f;
    [SerializeField] private float testShakeDuration = .1f;
    
    private Transform _cameraTransform; // Reference to the camera transform
    private const float ShakeIntensityDefault = 0.025f;
    private const float ShakeDurationDefault = 0.25f;
    private const float RotationIntensity = 0.05f;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        _cameraTransform = GetComponent<Transform>();
    }

    private void Start()
    {
        // Store the original position of the camera
        _originalPosition = _cameraTransform.localPosition;
        _originalRotation = _cameraTransform.localRotation;
    }

    [Command]
    private void DebugShake()
    {
        StartCameraShake(testShakeIntensity, testShakeDuration);
    }
    
    public void StartCameraShake(float intensity = ShakeIntensityDefault, float duration = ShakeDurationDefault)
    {
        // If there's already a shake running, stop it before starting a new one
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            _cameraTransform.localPosition = _originalPosition; // Reset position in case previous shake was interrupted
        }

        // Start a new shake with the given intensity and duration
        _shakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    // Coroutine to handle the screen shake
    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        float elapsedTime = 0f;

        // Keep shaking until the duration is over
        while (elapsedTime < duration)
        {
            // Apply a random shake by changing the camera position
            _cameraTransform.localPosition = _originalPosition + Random.insideUnitSphere * intensity;

            // Apply a random shake to the rotation
            Quaternion randomRotation = Quaternion.Euler(
                Random.Range(-RotationIntensity, RotationIntensity), // X-axis rotation
                Random.Range(-RotationIntensity, RotationIntensity), // Y-axis rotation
                Random.Range(-RotationIntensity, RotationIntensity) // Z-axis rotation
            );
            _cameraTransform.localRotation = _originalRotation * randomRotation;

            elapsedTime += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }

        RestoreDefaults();

        // Clear the coroutine reference
        _shakeCoroutine = null;
    }

    private void RestoreDefaults()
    {
        _cameraTransform.localPosition = _originalPosition;
        _cameraTransform.localRotation = _originalRotation;
    }

    public void EndCameraShake()
    {
        RestoreDefaults();
        StopAllCoroutines();
    }
}