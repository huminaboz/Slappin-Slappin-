using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public abstract class Enemy : MonoBehaviour, IHpAdjustmentListener, IObjectPool<Enemy>
{
    private IHpAdjustmentListener _hpAdjustmentListenerImplementation;
    public Health thisHealth { get; set; }

    [SerializeField] public float spawnChance = .15f;

    public virtual void SetupObjectFirstTime()
    {
        gameObject.SetActive(false);
        thisHealth = GetComponent<Health>();
    }

    public virtual void InitializeObjectFromPool()
    {
        thisHealth.Initialize();
        gameObject.SetActive(true);
    }
    
    public void TookDamage(int damageAmount, GameObject attacker)
    {
    }

    public void Healed(int healAmount, GameObject healer)
    {
    }

    public virtual float HandleDeath(int lastAttack, GameObject killer)
    {
        //Throw up a puff of particle
        //Return to the pool
        ReturnObjectToPool();
        return 0;
    }



    public virtual void ReturnObjectToPool()
    {
        //thisHealth = null;
    }
}