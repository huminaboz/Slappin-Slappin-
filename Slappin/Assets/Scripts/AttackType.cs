using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackType : MonoBehaviour
{
    [SerializeField] protected SO_AttackData attackData;

    public virtual void HitSomething(GameObject thingThatGotHit)
    {
        //If hitting something with health, do damage to it
        if (thingThatGotHit.GetComponent(typeof(Health)) != null)
        {
            Health health = thingThatGotHit.GetComponent<Health>();
            health.AdjustHp(-attackData.baseDamage, gameObject);
        }
    }
}
