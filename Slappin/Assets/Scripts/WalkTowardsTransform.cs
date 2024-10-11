using System.Collections;
using UnityEngine;

public class WalkTowardsTransform : MonoBehaviour, IHpAdjustmentListener
{
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5f;

    private Enemy thisEnemy;

    //TODO:: Can set this up to target a random X position at the hurt line

    private void Awake()
    {
        if (TryGetComponent(out Enemy enemy))
        {
            thisEnemy = enemy;
        }
        else Debug.LogError("Walk script doesn't have an enemy script");
    }

    private void Start()
    {
        if (EnemyTarget.I.targetTransform == null)
        {
            Debug.LogError("target transform is null");
        }
    }

    private void Update()
    {
        if (transform.localPosition.z + transform.localScale.z * .05 <= EnemyTarget.I.hurtLine.localPosition.z
            && walkSpeed > 0f)
        {
            //TODO:: Turn off this component and inform the enemy it's time to attack
            thisEnemy.SwitchToAttackMode();
            return;
        }

        // Get the direction from the current position to the target position
        Vector3 direction = (EnemyTarget.I.targetTransform.position - transform.position);
        direction.y = 0f; // Ignore the Y axis for movement

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

    public void BackUp()
    {
        walkSpeed *= -10;
        StartCoroutine(SwitchDirectionsTimer());
    }

    private void ReturnToForward()
    {
        walkSpeed *= -.1f;
    }

    IEnumerator SwitchDirectionsTimer()
    {
        yield return new WaitForSeconds(.5f);
        ReturnToForward();
    }

    public void TookDamage(int damageAmount, GameObject attacker)
    {
    }

    public void Healed(int healAmount, GameObject healer)
    {
    }

    public float HandleDeath(int lastAttack, GameObject killer)
    {
        StopAllCoroutines();
        
        return 0;
    }
}