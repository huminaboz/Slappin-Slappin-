using UnityEngine;
using UnityEngine.Serialization;

public class AttackType : MonoBehaviour
{
    [SerializeField] protected SO_AttackData attackData;
    [SerializeField] protected Player player;

    protected Color defaultBottomOfHandColor;
    [FormerlySerializedAs("slapRenderer")] [SerializeField] private Renderer handRenderer;
    private Material handMaterial;
    [SerializeField] protected Rigidbody _handRigidbody;


    
    private void Awake()
    {
    }

    public virtual void DoAttack()
    {
        GetHandMaterials();
    }

    private void GetHandMaterials()
    {
        //TODO:: Move this off to another script that's involved in the visuals of the hands
        defaultBottomOfHandColor = handRenderer.material.GetColor("_ColorDimExtra");
        handMaterial = handRenderer.material;
    }

    public void HandleGettingHurt()
    {
        handMaterial.SetColor("_ColorDimExtra", Color.red);
    }

    public void RestoreDefaultAppearance()
    {
        handRenderer.material.SetColor("_ColorDimExtra", defaultBottomOfHandColor);
    }

    public void HitSomething(GameObject thingThatGotHit)
    {
        //HURT IT!
        if (thingThatGotHit.GetComponent<Health>() != null)
        {
            Health health = thingThatGotHit.GetComponent<Health>();
            int damage = GetBonusDamage(attackData.baseDamage);
            health.AdjustHp(-damage, gameObject);
            if (-attackData.baseDamage < 0)
            {
                SFXPlayer.I.Play(attackData.playSFXOnHit);
            }
        }
    }

    private int GetBonusDamage(int baseDamage)
    {
        float bonusDamage = baseDamage;
        //TODO:: Attach a color for the damage number
        float roll = BozUtilities.GetDiceRoll();

        if (roll <= .5f)
        {
            return baseDamage;
        }
        
        if (roll > .5f)
        {
            bonusDamage = Mathf.Max(baseDamage * attackData.bonus_goodHit, baseDamage+1);    
        }

        if (roll > .8f)
        {
            bonusDamage = Mathf.Max(baseDamage * attackData.bonus_greatHit, baseDamage+2);    
        }

        if (roll > .95f)
        {
            bonusDamage = Mathf.Max(baseDamage * attackData.bonus_criticalHit, baseDamage+5);    
        }

        if (roll > .99f)
        {
            bonusDamage = Mathf.Max(baseDamage * attackData.bonus_legendaryHit, bonusDamage+8);    
        }

        return (int) bonusDamage;
    }

    public void TookDamage(int damageAmount, GameObject attacker)
    {
        throw new System.NotImplementedException();
    }

    public void Healed(int healAmount, GameObject healer)
    {
        throw new System.NotImplementedException();
    }

    public float HandleDeath(int lastAttack, GameObject killer)
    {
        throw new System.NotImplementedException();
    }
}
