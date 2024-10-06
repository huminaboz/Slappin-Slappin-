using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [ReadOnly] private int hp;
    [SerializeField] private int maxHp = 5;
    [SerializeField] private UnityEvent OnDeath;

    public void AdjustHp(int amount, IDamageable damageable)
    {
        hp += amount;
        if (hp < 0)
        {
            HandleDeath(damageable);    
        }
    }

    private void HandleDeath(IDamageable damageable)
    {
        damageable.HandleDeath();
    }
    
}
