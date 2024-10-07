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
        //Got hit feedback
    }

    public void Healed(int healAmount, GameObject healer)
    {
        //Got healed feedback
    }

    public virtual float HandleDeath(int lastAttack, GameObject killer)
    {
        //Throw up a puff of particle
        ReturnObjectToPool();
        return 0;
    }



    //Abstract because you don't want to return the object to the pool as an Enemy, but as the specific enemy
    public abstract void ReturnObjectToPool();
}