using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Health))]
public abstract class Enemy : MonoBehaviour, IHpAdjustmentListener, IObjectPool<Enemy>
{
    private IHpAdjustmentListener _hpAdjustmentListenerImplementation;
    public Health thisHealth { get; set; }

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
        //TODO: Throw up a puff of particle
        
        //Spin in a circle first
        transform.DORotate(new Vector3(0, 720, 0), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .OnComplete(ReturnObjectToPool);

        return 0;
    }



    //Abstract because you don't want to return the object to the pool as an Enemy, but as the specific enemy
    public abstract void ReturnObjectToPool();
}