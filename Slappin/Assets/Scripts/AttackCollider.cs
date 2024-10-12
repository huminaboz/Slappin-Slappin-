using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private AttackType _attackType;

    private void OnTriggerEnter(Collider other)
    {
        _attackType.HitSomething(other.gameObject);
    }
}