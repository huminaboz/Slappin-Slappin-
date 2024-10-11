using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private AttackType _attackType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent(typeof(Health)) != null)
        {
            Health health = other.GetComponent<Health>();
            _attackType.AddHitHealth(health);
            // _attackType.HitSomething(other.gameObject);
        }
    }
}