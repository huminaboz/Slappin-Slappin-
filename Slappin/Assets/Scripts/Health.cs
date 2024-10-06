using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [field: ReadOnly] public int Hp { get; private set; }
    [SerializeField] private int maxHp = 5;
    [SerializeField] private UnityEvent OnDeath;

    private void OnEnable()
    {
        Hp = maxHp;
    }

    public void AdjustHp(int amount, IDamageable damageable)
    {
        Hp += amount;
        if (Hp <= 0)
        {
            HandleDeath(damageable);    
        }
    }

    private void HandleDeath(IDamageable damageable)
    {
        damageable.HandleDeath();
    }
    
}
