using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private AttackType _attackType;
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning($"{_attackType} hit {other}");
        if (other.GetComponent<Enemy_Spike>() is not null) return;
        _attackType.HitSomething(other.gameObject);
        if (_attackType is SquishAttack)
        {
            enabled = false;
        }
    }
}