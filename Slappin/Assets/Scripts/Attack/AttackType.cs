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
            health.AdjustHp(-attackData.baseDamage, gameObject);
            if (-attackData.baseDamage < 0)
            {
                SFXPlayer.I.Play(attackData.playSFXOnHit);
            }
        }
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
