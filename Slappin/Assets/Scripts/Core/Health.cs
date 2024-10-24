using System;
using QFSW.QC;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Health : MonoBehaviour
{
    [SerializeField] private string notes = "MaxHp is set by upgrades on scrobs";
    [ReadOnly] public int hp;
    [SerializeField] public int maxHp = 5;
    [FormerlySerializedAs("enemyMaxHp")] public int enemyBaseMaxHp;

    public static Action OnDeath;

    // private bool immuneToDamage = false;
    public bool isAlive = true;
    public bool isPlayer = false;
    [SerializeField] private bool isInvincible = false;

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
        if (!isPlayer) return;
        int previousMaxHp = maxHp;
        maxHp = (int)StatLiason.I.Get(Stat.IncreaseMaxHp);
        if (previousMaxHp == maxHp) return;
        int hpUpgrade = maxHp - previousMaxHp;
        Debug.Log($"Increased maxHp by: {hpUpgrade}. New value is {maxHp}");
        AdjustHp(hpUpgrade, null);
    }

    public void Initialize()
    {
        if (isInvincible) Debug.LogError($"{gameObject.name} is invincible!!");
        if (isPlayer) maxHp = (int)StatLiason.I.Get(Stat.IncreaseMaxHp);
        else
        {
            float hpMultiplier = StatLiason.I.GetEnemy(Stat.Enemy_MaxHp);
            maxHp = (int)(enemyBaseMaxHp * hpMultiplier);
        }

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
            //TODO:: If attacker is null, don't play a celebration
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
            if (isInvincible)
            {
                hp = maxHp;
                return;
            }

            isAlive = false;

            float maxWaitTime = 0;
            foreach (IHpAdjustmentListener damageListeners in hpAdjustmentListeners)
            {
                // Debug.Log($"{damageListeners} is handling death.");
                maxWaitTime = Mathf.Max(damageListeners.HandleDeath(amountToIncrease, attacker), maxWaitTime);
            }

            if (isPlayer) OnDeath?.Invoke();
            // StartCoroutine(QueueCleanup(maxWaitTime + 0.1f));
        }
    }

    [Command]
    private void DebugKillAllEnemies()
    {
        if (!isPlayer) AdjustHp(-maxHp, null);
    }

    [Command]
    private void DebugKillPlayer()
    {
        if (isPlayer) AdjustHp(-maxHp, null);
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