using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class HandMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f; // Default movement speed
    // [SerializeField] private float boostedSpeedMultiplier = 2f; // How much faster when holding the right trigger
    [SerializeField] private Transform handPositioner;
    [SerializeField] private Player thisPlayer;
    
    [HideInInspector] public Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        _rigidbody.velocity = Vector3.zero;
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
        float currentSpeed = isBoosting ? moveSpeed * StatLiason.I.Get(Stat.MoveBoostSpeed) : moveSpeed;
        // Debug.LogWarning($"Current Speed: {currentSpeed}");

        // Apply movement to the player transform
        if (direction.magnitude >= 0.1f)
        {
            _rigidbody.velocity = direction * (currentSpeed * Time.deltaTime);
            thisPlayer.CurrentAttackType?.SetFacingDirection();
        }
        else
        {
            _rigidbody.velocity = Vector3.zero;
        }
    }
}