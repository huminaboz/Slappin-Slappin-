using System;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackType : MonoBehaviour
{
    [SerializeField] public Transform handPositioner;
    [SerializeField] public GameObject hurtEnemiesColliderObject;
    
    [HideInInspector] public Vector3 offsetPosition;

    [SerializeField] protected SO_AttackData attackData;
    [SerializeField] protected Player player;
    [SerializeField] protected Rigidbody handRigidbody;
    [SerializeField] protected float offScreenSlapYPosition = -0.23f; //Measured by holding it off camera
    [SerializeField] protected float groundYPosition = -0.78f; //Measured by putting the hand on the ground
    [FormerlySerializedAs("descentCurve")] [FormerlySerializedAs("startSlapCurve")] [SerializeField] protected AnimationCurve movementCurve;
    [SerializeField] protected GameObject pickupColliderObject;
    [SerializeField] protected Health playerHealth;
    protected float attackSpeed;
    protected const float distanceFlexRoom = .05f;
    protected Vector3 goalPosition;
    protected Action OnCompletedTravel;
    protected float startingDistanceFromGoal;
    protected Vector3 direction;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Renderer handRenderer;
    [SerializeField] private GameObject handModel;
    [SerializeField] private Transform handShadowTransform;
    
    private Color _defaultBottomOfHandColor;
    private Material _handMaterial;
    private Vector3 relativeAttackPositioning;
    private Vector3 storedRelativeAttackPosition;
    
    public void Initialize()
    {
        GetHandMaterials();

        //Every attack type is going to have different relative positioning because of the pivots and visuals
        relativeAttackPositioning = handPositioner.position - handShadowTransform.position;
        Debug.Log($"Relative positioning between shadow and hand is: {relativeAttackPositioning}");
        storedRelativeAttackPosition = relativeAttackPositioning;
        handModel.SetActive(false);
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

    public virtual void InitiateAttack()
    {
        if (!playerHealth.isAlive)
        {
            Debug.LogWarning("Can't attack - player is dead");
            return;
        }
        SetPosition(); //So the spike collision check is in the right place
        SetDirection();
        handModel.SetActive(true);
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

    protected virtual void Cleanup()
    {
        handRigidbody.velocity = Vector3.zero;
        direction = Vector3.zero;

        handModel.SetActive(false);
        player.SetState(new StateDefault(player));
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