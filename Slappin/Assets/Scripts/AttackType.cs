using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackType : MonoBehaviour
{
    [SerializeField] protected SO_AttackData attackData;

    protected List<Health> victimsHit = new List<Health>();
    protected List<Pickup> pickupsHit = new List<Pickup>();
    
    public void AddHitHealth(Health health)
    {
        victimsHit.Add(health);
    }

    public void AddHitPickup(Pickup pickup)
    {
        pickupsHit.Add(pickup);
    }

    protected void DumpCollisionsLists()
    {
        victimsHit.Clear();
        pickupsHit.Clear();
    }
    
    public virtual void HitVictim(Health thingThatGotHit)
    {
        thingThatGotHit.AdjustHp(-attackData.baseDamage, gameObject);
    }

    public virtual void PickSomethingUp(Pickup pickup)
    {
        pickup.GetPickedUp();
    }
    
    protected void HandleCollisions()
    {
        foreach (Health health in victimsHit)
        {
            HitVictim(health);
        }

        foreach (Pickup pickup in pickupsHit)
        {
            PickSomethingUp(pickup);
        }
    }
}
