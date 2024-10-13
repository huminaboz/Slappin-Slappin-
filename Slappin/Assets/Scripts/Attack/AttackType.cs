using UnityEngine;
using UnityEngine.Serialization;

public class AttackType : MonoBehaviour
{
    [SerializeField] protected SO_AttackData attackData;
    [SerializeField] protected Player player;

    [SerializeField] public Transform handPositioner;

    [SerializeField] private Renderer handRenderer;

    [SerializeField] protected Rigidbody handRigidbody;

    [SerializeField] private float centerX = .29f;

    [SerializeField] public GameObject hurtEnemiesColliderObject;


    private Color _defaultBottomOfHandColor;
    private Material _handMaterial;

    [SerializeField] private Transform handShadowTransform;
    [SerializeField] private Transform cameraTransform;
    private Vector3 relativeAttackPositioning;
    [HideInInspector] public Vector3 offsetPosition;
    private Vector3 storedRelativeAttackPosition;
    
    public void Initialize()
    {
        GetHandMaterials();

        //Every attack type is going to have different relative positioning because of the pivots and visuals
        relativeAttackPositioning = handPositioner.position - handShadowTransform.position;
        Debug.Log($"Relative positioning between shadow and hand is: {relativeAttackPositioning}");
        storedRelativeAttackPosition = relativeAttackPositioning;
    }

    private void SetDirection()
    {
        //If on the right side of the center line, reverse the x scale
        if (handPositioner.position.x > cameraTransform.position.x)
        {
            //FLIP IT
            handPositioner.localScale = new Vector3(
                -1f * Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);
            
            //Flip the X of the relative attack position as well
            relativeAttackPositioning = new Vector3(-storedRelativeAttackPosition.x,
                storedRelativeAttackPosition.y, storedRelativeAttackPosition.z);
        }
        else
        {
            //DONT FLIP IT
            handPositioner.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);
            relativeAttackPositioning = storedRelativeAttackPosition;
        }
    }

    public void SetPosition()
    {
        //Move the hand with the shadow only on X and Z
        offsetPosition = handShadowTransform.position + relativeAttackPositioning;

        handPositioner.position = new Vector3(offsetPosition.x,
            handPositioner.position.y, offsetPosition.z);
    }

    public virtual void DoAttack()
    {
        SetPosition(); //So the spike collision check is in the right place
        SetDirection();
    }

    private void GetHandMaterials()
    {
        //TODO:: Move this off to another script that's involved in the visuals of the hands
        _defaultBottomOfHandColor = handRenderer.material.GetColor("_ColorDimExtra");
        _handMaterial = handRenderer.material;
    }

    public void HandleGettingHurt()
    {
        _handMaterial.SetColor("_ColorDimExtra", Color.red);
    }

    public void RestoreDefaultAppearance()
    {
        handRenderer.material.SetColor("_ColorDimExtra", _defaultBottomOfHandColor);
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
            bonusDamage = Mathf.Max(baseDamage * attackData.bonus_goodHit, baseDamage + 1);
        }

        if (roll > .8f)
        {
            bonusDamage = Mathf.Max(baseDamage * attackData.bonus_greatHit, baseDamage + 2);
        }

        if (roll > .95f)
        {
            bonusDamage = Mathf.Max(baseDamage * attackData.bonus_criticalHit, baseDamage + 5);
        }

        if (roll > .99f)
        {
            bonusDamage = Mathf.Max(baseDamage * attackData.bonus_legendaryHit, bonusDamage + 8);
        }

        return (int)bonusDamage;
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