using UnityEngine;

public class AttackType : MonoBehaviour
{
    [SerializeField] protected SO_AttackData attackData;


    public void HitSomething(GameObject thingThatGotHit)
    {
        if (thingThatGotHit.GetComponent<Health>() != null)
        {
            Health health = thingThatGotHit.GetComponent<Health>();
            health.AdjustHp(-attackData.baseDamage, gameObject);
            if (-attackData.baseDamage < 0)
            {
                SFXPlayer.I.Play(attackData.playSFXOnHit);
            }
        }
    }
}
