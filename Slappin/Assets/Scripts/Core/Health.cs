using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [ReadOnly] public int hp;
    [SerializeField] public int maxHp = 5;
    [SerializeField] private UnityEvent OnDeath;
    // private bool immuneToDamage = false;
    public bool isAlive = true;
    public bool isPlayer = false;

    private IHpAdjustmentListener[] hpAdjustmentListeners;

    private void Awake()
    {
        //This is so anything we need to inform of this object's untimely demise can know
        hpAdjustmentListeners = GetComponents<IHpAdjustmentListener>();
    }

    private void OnEnable()
    {
        UpgradeData.OnPurchaseMade += BoughtHp;
    }

    private void OnDisable()
    {
        UpgradeData.OnPurchaseMade -= BoughtHp;
    }

    private void BoughtHp()
    {
        if(!isPlayer) return;
        int previousMaxHp = maxHp;
        maxHp = (int) StatLiason.I.Get(Stat.IncreaseMaxHp);
        int hpUpgrade = maxHp - previousMaxHp;
        Debug.Log($"Increased maxHp by: {hpUpgrade}. New value is {maxHp}");
        AdjustHp(hpUpgrade, null);
    }

    public void Initialize()
    {
        if (isPlayer) maxHp = (int) StatLiason.I.Get(Stat.IncreaseMaxHp);
        //TODO:: Get the enemy's max hp stat
        hp = maxHp;
        isAlive = true;
    }

    public void AdjustHp(int amountToIncrease, GameObject attacker)
    {
        if (!isAlive) return;

        if (isPlayer && amountToIncrease < 0)
        {
            float adjustedDamage = amountToIncrease - amountToIncrease 
                * StatLiason.I.Get(Stat.DamageReduction);
            Debug.Log($"{StatLiason.I.Get(Stat.DamageReduction)} " +
                      $"Damage Reduction shaved off {amountToIncrease - adjustedDamage} damage" +
                      $"\nFrom {amountToIncrease} to {adjustedDamage}");
            amountToIncrease = Mathf.CeilToInt(adjustedDamage);
        }
        
        int oldHealth = hp;
        hp += amountToIncrease;
        
        if (oldHealth == hp) return;
        
        Debug.Log($"{gameObject.name} health adjusted by {amountToIncrease}. "
                  + $"\nHp is now {hp}");

        
        //HEALING
        if (oldHealth < hp)
        {
            //TODO:: If attacker is nyull, don't play a celebration
            foreach (IHpAdjustmentListener damageListeners in hpAdjustmentListeners)
            {
                damageListeners.Healed(amountToIncrease, attacker);
            }
            return;
        }

        //HURTING
        if (oldHealth > hp)
        {
            foreach (IHpAdjustmentListener damageListeners in hpAdjustmentListeners)
            {
                damageListeners.TookDamage(amountToIncrease, attacker);
            }
        }

        //DEADING
        if (hp <= 0)
        {
            isAlive = false;

            float maxWaitTime = 0;
            foreach (IHpAdjustmentListener damageListeners in hpAdjustmentListeners)
            {
                // Debug.Log($"{damageListeners} is handling death.");
                maxWaitTime = Mathf.Max(damageListeners.HandleDeath(amountToIncrease, attacker), maxWaitTime);
            }

            OnDeath?.Invoke();
            // StartCoroutine(QueueCleanup(maxWaitTime + 0.1f));
        }
    }

    // IEnumerator QueueCleanup(float waitTime)
    // {
    //     yield return waitTime;
    //     Cleanup();
    // }
    //
    // private void Cleanup()
    // {
    //     gameObject.SetActive(false);
    // }
}