using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [ReadOnly] public int hp;
    [SerializeField] private int maxHp = 5;
    [SerializeField] private UnityEvent OnDeath;

    private void OnEnable()
    {
        hp = maxHp;
    }

    public void AdjustHp(int amount, IDamageable damageable)
    {
        hp += amount;
        Debug.Log($"{gameObject.name} damaged for {amount}. "
                  + $"\nHp is now {hp}");
        if (hp <= 0)
        {
            HandleDeath(damageable);    
        }
    }

    private void HandleDeath(IDamageable damageable)
    {
        OnDeath?.Invoke();
        damageable.HandleDeath();
    }
    
}
