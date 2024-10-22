using System;
using UnityEngine;

public class DoTCollider : MonoBehaviour
{
    private float damageRate;
    private float damage;


    private void OnEnable()
    {
        UpgradeData.OnPurchaseMade += UpdateStats;
    }

    private void OnDisable()
    {
    }

    private void Start()
    {
        UpdateStats();
    }

    private void UpdateStats()
    {
        Debug.LogWarning($"Stat.SquishDamgeOverTime {StatLiason.I.Get(Stat.SquishDamgeOverTime)}");
        damageRate = StatLiason.I.Get(Stat.SquishDotRate);
        damage = StatLiason.I.Get(Stat.SquishDamgeOverTime);
    }

    private float t = 0f;

    private void OnTriggerEnter(Collider other)
    {
        HurtHealth(other);
    }

    private void HurtHealth(Collider other)
    {
        if (other.GetComponent<Health>() != null)
        {
            UpdateStats();
            Health health = other.GetComponent<Health>();
            health.AdjustHp(-(int)damage, gameObject);
            if (-damage < 0)
            {
                SFXPlayer.I.Play(AudioEventsStorage.I.dotHit);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Health>() == null) return;
        t += Time.deltaTime;
        if (t >= damageRate)
        {
            if (other.GetComponent<Enemy_Spike>() is not null)
            {
                SFXPlayer.I.Play(AudioEventsStorage.I.bouncerBlocked);
                return;
            }

            Health health = other.GetComponent<Health>();
            health.AdjustHp(-(int)damage, gameObject);
            if (-damage < 0)
            {
                SFXPlayer.I.Play(AudioEventsStorage.I.dotHit);
            }

            t = 0f;
        }
    }
}