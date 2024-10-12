using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private AttackType _attackType;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Enemy_Spike>() is not null) return;
        _attackType.HitSomething(other.gameObject);
    }
}