using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveTowardsTransform : MonoBehaviour, IHpAdjustmentListener
{
    [FormerlySerializedAs("walkSpeed")] [SerializeField] public float baseWalkSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5f;

    [HideInInspector] public float walkSpeed;
    
    private Enemy thisEnemy;
    private Rigidbody _rigidbody;
    public bool isDashing = false;

    //TODO:: Can set this up to target a random X position at the hurt line

    private void Awake()
    {
        if (TryGetComponent(out Enemy enemy))
        {
            thisEnemy = enemy;
        }
        else Debug.LogError("Walkscript doesn't have an enemy script");

        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (EnemyTarget.I.targetTransform == null)
        {
            Debug.LogError("target transform is null");
        }
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public bool IsInAttackRange()
    {
        return transform.localPosition.z + transform.localScale.z * .05
               <= EnemyTarget.I.hurtLine.localPosition.z;
    }

    private void FixedUpdate()
    {
        if (!thisEnemy.thisHealth.isAlive) return;
        if (IsInAttackRange() && walkSpeed > 0f)
        {
            _rigidbody.velocity = Vector3.zero;
            thisEnemy.SwitchToAttackMode();
            return;
        }

        // Get the direction from the current position to the target position
        Vector3 direction = EnemyTarget.I.targetTransform.position - transform.position;
        direction.y = 0f;

        _rigidbody.velocity = direction.normalized * (walkSpeed * Time.deltaTime);

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
        //TODO:: Make this not so depend on the speed and more so on the distance traveled
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
        _rigidbody.velocity = Vector3.zero;
        return 0;
    }
}