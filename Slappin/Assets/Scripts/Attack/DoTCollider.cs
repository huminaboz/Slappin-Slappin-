using System;
using UnityEngine;

public class DoTCollider : MonoBehaviour
{
    private float damageRate;
    private float damage;

    private void OnEnable()
    {
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
        if (other.GetComponent<Enemy_Spike>() is not null) return;

        if (other.GetComponent<Health>() == null) return;
        t += Time.deltaTime;
        if (t >= damageRate)
        {
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