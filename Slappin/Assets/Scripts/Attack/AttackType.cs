using System;
using QFSW.QC.Actions;
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
    [SerializeField] protected float offScreenHandYPosition = -0.23f; //Measured by holding it off camera
    [SerializeField] protected float groundYPosition = -0.78f; //Measured by putting the hand on the ground
    [SerializeField] protected AnimationCurve movementCurve;
    [SerializeField] protected Health playerHealth;
    [SerializeField] protected Transform cameraTransform;
    [SerializeField] protected Renderer handRenderer;

    protected Action OnCompletedTravel;

    protected Vector3 Direction
    {
        get => direction;
        set { direction = value; }
    }

    private Vector3 direction;
    protected Color _defaultTopOfHandColor;

    [SerializeField] private Transform handShadowTransform;

    private float attackSpeed;
    private const float distanceFlexRoom = .05f;
    private Vector3 goalPosition;
    private float startingDistanceFromGoal;
    private Color _defaultBottomOfHandColor;
    private Material _handMaterial;
    private Vector3 relativeAttackPositioning;
    private Vector3 storedRelativeAttackPosition;

    private void Start()
    {
        handPositioner.position = new Vector3(handPositioner.position.x,
            offScreenHandYPosition, handPositioner.position.z);
    }

    private void FixedUpdate()
    {
        // Debug.LogWarning(handRigidbody.velocity);

        if (Direction == Vector3.zero) return;
        float YDistance = Mathf.Abs(handPositioner.position.y - goalPosition.y);
        //TODO:: Different curve for going up and for going down
        //TODO:: Can't let starting distance from goal end up as zero
        float ratio = 1f - movementCurve.Evaluate(YDistance / startingDistanceFromGoal);
        ratio = Mathf.Clamp(ratio, .05f, 1f); //Don't let it be 0

        handRigidbody.velocity = Direction * (Time.fixedDeltaTime * attackSpeed * ratio);

        if (direction.y > 0)
        {
            //heading up
            if (handPositioner.transform.position.y >= goalPosition.y)
            {
                DoWhenMadeItToGoalPosition();
            }
        }
        else
        {
            //heading down
            if (handPositioner.transform.position.y <= goalPosition.y)
            {
                DoWhenMadeItToGoalPosition();
            }
        }

        // Keep this around as the old way that caused some problems in case the new way causes problems
        //Could calculate the flex space by making it slightly greater than the speed time deltafixed time, but would have to set an offset position as well
        // if (YDistance <= distanceFlexRoom + attackData.goDownSpeed * .001f)
        // {
        //     DoWhenMadeItToGoalPosition();
        // }
    }

    private void DoWhenMadeItToGoalPosition()
    {
        StopTraveling();
        handPositioner.transform.position = new Vector3(handPositioner.position.x,
            goalPosition.y, handPositioner.position.z);
        OnCompletedTravel?.Invoke();
    }

    public virtual void Initialize()
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

    public void SetParentPosition()
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

        SetParentPosition(); //So the spike collision check is in the right place
        SetDirection();
    }

    protected virtual void InitiateTravelToGround()
    {
        goalPosition = new(transform.position.x, groundYPosition, transform.position.z);

        OnCompletedTravel += DoWhenReachingGround;

        attackSpeed = attackData.goDownSpeed;
        startingDistanceFromGoal = Mathf.Abs(handPositioner.position.y - goalPosition.y);

        //Giving Direction a value starts up the Fixedupdate telling the hand which way to go
        Direction = new(0, -1, 0);
    }

    private void StopTraveling()
    {
        Direction = Vector3.zero;
        handRigidbody.velocity = Vector3.zero;
    }

    protected virtual void InitiateTravelBackUp()
    {
        StopTraveling();

        goalPosition = new(transform.position.x, offScreenHandYPosition, transform.position.z);
        startingDistanceFromGoal = Mathf.Abs(handPositioner.position.y - goalPosition.y);
        attackSpeed = attackData.goBackUpSpeed;
        Direction = new(0, 1, 0);

        OnCompletedTravel = Cleanup;
    }

    protected virtual void DoWhenReachingGround()
    {
        StopTraveling();
    }

    private void GetHandMaterials()
    {
        //TODO:: Move this off to another script that's involved in the visuals of the hands
        _defaultBottomOfHandColor = handRenderer.material.GetColor("_ColorDimExtra");
        _defaultTopOfHandColor = handRenderer.material.GetColor("_ColorDim");
        _handMaterial = handRenderer.material;
    }

    public void HandleGettingHurt()
    {
        _handMaterial.SetColor("_ColorDimExtra", Color.red);
    }

    public void RestoreDefaultAppearance()
    {
        handRenderer.material.SetColor("_ColorDimExtra", _defaultBottomOfHandColor);
        handRenderer.material.SetColor("_ColorDim", _defaultTopOfHandColor);
    }

    public virtual void HitSomething(GameObject thingThatGotHit)
    {
        //HURT IT!
        if (thingThatGotHit.GetComponent<Health>() != null)
        {
            Health health = thingThatGotHit.GetComponent<Health>();
            int damage = GetBonusDamage(attackData.baseDamage);
            health.AdjustHp(-damage, gameObject);
            if (-damage < 0)
            {
                SFXPlayer.I.Play(attackData.playSFXOnHit);
            }
        }
    }

    protected virtual void Cleanup()
    {
        StopTraveling();
        OnCompletedTravel = null;
        player.SetState(new StateDefault(player));
    }

    public int GetBonusDamage(int baseDamage)
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