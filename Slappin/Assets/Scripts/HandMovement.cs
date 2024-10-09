using UnityEngine;

public class HandMovement : MonoBehaviour, IHpAdjustmentListener
{
    [SerializeField] private float moveSpeed = 2.5f; // Default movement speed
    [SerializeField] private float boostedSpeedMultiplier = 2f; // How much faster when holding the right trigger
    [SerializeField] private Transform slapHandObject;
    
    private void Update()
    {
        if (!StateGame.PlayerInGameControlsEnabled) return;
        
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
            
            //Move the slap hand with the shadow only on X and Z 
            slapHandObject.position = new Vector3(transform.position.x, slapHandObject.position.y, transform.position.z);
        }
    }

    public void TookDamage(int damageAmount, GameObject attacker)
    {
    }

    public void Healed(int healAmount, GameObject healer)
    {
    }

    public float HandleDeath(int lastAttack, GameObject killer)
    {
        enabled = false;
        return 0;
    }
}