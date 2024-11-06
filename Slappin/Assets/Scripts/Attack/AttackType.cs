using System;
using System.Collections;
using QFSW.QC.Actions;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackType : MonoBehaviour
{
    [SerializeField] public Transform handPositioner;
    [SerializeField] public GameObject hurtEnemiesColliderObject;
    [SerializeField] public Rigidbody handRigidbody;

    [HideInInspector] public Vector3 offsetPosition;

    [SerializeField] protected SO_AttackData attackData;
    [SerializeField] protected Player player;
    [SerializeField] protected float offScreenHandYPosition = -0.23f; //Measured by holding it off camera
    [SerializeField] protected float groundYPosition = -0.78f; //Measured by putting the hand on the ground
    [SerializeField] protected AnimationCurve movementCurve;
    [SerializeField] protected Health playerHealth;
    [SerializeField] protected Transform cameraTransform;
    [SerializeField] protected Renderer handRenderer;

    protected Action OnCompletedTravel;

    //Calculating Range Damage Bonus
    [SerializeField] protected AnimationCurve rangeDamageCurve;
    [SerializeField] protected Transform spawnerLine;
    [SerializeField] protected Transform hurtLine;
    
    
    protected Vector3 Direction
    {
        get => direction;
        set { direction = value; }
    }

    private Vector3 direction;
    protected Color _defaultTopOfHandColor;

    [SerializeField] private Transform handShadowTransform;

    private float attackSpeed = 0f;
    private const float distanceFlexRoom = .05f;
    protected Vector3 goalPosition;
    private float startingDistanceFromGoal;
    private Color _defaultBottomOfHandColor;
    private Material _handMaterial;
    private Vector3 relativeAttackPositioning;
    private Vector3 storedRelativeAttackPosition;
    
    [SerializeField] protected Animator animator;

    private void OnEnable()
    {
        UIStateSwapper.OnEnterStore += CleanupThatsOnlyCalledFromStateMachine;
    }

    private void OnDisable()
    {
        UIStateSwapper.OnEnterStore -= CleanupThatsOnlyCalledFromStateMachine;
    }

    private void Start()
    {
        SetDefaultPosition();
    }

    private void SetDefaultPosition()
    {
        Debug.LogWarning($"{attackData.title} setting default position");
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
        if (startingDistanceFromGoal == 0f) return;
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

        //Could also initially grab the total distance you need to travel and then just compare to if you're past that
    }


    //Be sure to use this in addition to and not multiplied by the damage
    protected float GetRangedDamageBonus(float damagePerDistance)
    {
        //Hurtline Z is 0% distance traveled
        //Spawnline Z is 100% damage traveled
        //Current position Z is X distance

        float z1 = hurtLine.position.z;
        float z2 = spawnerLine.position.z;
        float pointZ = handShadowTransform.position.z;

        float normalizedPosition = (pointZ - z1) / (z2 - z1);
        float curveAdjustedRatio = rangeDamageCurve.Evaluate(normalizedPosition);
        float rangeDamageBonus = curveAdjustedRatio * damagePerDistance;
        // Debug.LogWarning($"Ranged Damage Bonus: {rangeDamageBonus}");
        return rangeDamageBonus;
    }

    public virtual void HitSpike(GameObject spike)
    {
    }

    private void DoWhenMadeItToGoalPosition()
    {
        Debug.LogWarning($"{attackData.title} DoWhenMadeItToGoalPosition\n" +
                         $"Goalposition: {goalPosition}");
        StopTraveling();
        handPositioner.transform.position = new Vector3(handPositioner.position.x,
            goalPosition.y, handPositioner.position.z);
        OnCompletedTravel?.Invoke();
    }

    public virtual void Initialize()
    {
        GetHandMaterials();
        Debug.LogWarning($"{attackData.title} Initializing\n");
        //Every attack type is going to have different relative positioning because of the pivots and visuals
        relativeAttackPositioning = handPositioner.position - handShadowTransform.position;
        Debug.Log($"Relative positioning between shadow and hand is: {relativeAttackPositioning}");
        storedRelativeAttackPosition = relativeAttackPositioning;
    }

    public void SetFacingDirection()
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
        Debug.LogWarning($"{attackData.title} Initiate Attack");
        // if (!playerHealth.isAlive)
        // {
        //     Debug.LogWarning("Can't attack - player is dead");
        //     return;
        // }
        SetParentPosition(); //So the spike collision check is in the right place
        SetFacingDirection();
    }

    protected virtual void InitiateTravelToGround()
    {
        goalPosition = new(transform.position.x, groundYPosition, transform.position.z);
        Debug.LogWarning($"{attackData.title} InitiateTravelToGround\n" +
                         $"Goal Position: {goalPosition}");

        OnCompletedTravel += DoWhenReachingGround;

        attackSpeed = attackData.baseGoDownSpeed * StatLiason.I.Get(Stat.AttackSpeed);
        Debug.LogWarning($"{attackData.title} startingDistanceFromGoal {startingDistanceFromGoal}");
        startingDistanceFromGoal = Mathf.Abs(handPositioner.position.y - goalPosition.y);

        //Giving Direction a value starts up the Fixedupdate telling the hand which way to go
        Direction = new(0, -1, 0);
    }

    public void StopTraveling()
    {
        Direction = Vector3.zero;
        handRigidbody.velocity = Vector3.zero;
    }

    public virtual void InitiateTravelBackUp()
    {
        StopTraveling();
        goalPosition = new(transform.position.x, offScreenHandYPosition, transform.position.z);
        Debug.LogWarning($"{attackData.title} InitiateTravelBackUp: {goalPosition}");
        startingDistanceFromGoal = Mathf.Abs(handPositioner.position.y - goalPosition.y);
        Debug.LogWarning($"{attackData.title} startingDistanceFromGoal: {startingDistanceFromGoal}");
        attackSpeed = attackData.goBackUpSpeed * StatLiason.I.Get(Stat.AttackSpeed);
        Direction = new(0, 1, 0);

        OnCompletedTravel = CleanupAndSetDefaultState;
    }

    protected virtual void DoWhenReachingGround()
    {
        Debug.LogWarning($"{attackData.title} DoWhenReachingGround");
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
    
    private Coroutine _slapAnimationCoroutine;
    protected void PlayAnimationCoroutine(float delay, string animationName)
    {
        Debug.Log($"Starting animation: " + animationName);
        if(_slapAnimationCoroutine != null) StopCoroutine(_slapAnimationCoroutine);
        _slapAnimationCoroutine = StartCoroutine(PlayAnimation(delay, animationName));
    }
    private IEnumerator PlayAnimation(float delay, string animationName)
    {
        yield return new WaitForSeconds(delay);
            
        animator.Play(animationName);
    }

    protected virtual float GetAttackTypeDamageNumber()
    {
        Debug.LogError("Attack type should define it's own Damage Number with an override");
        return 0;
    }

    public virtual void HitSomething(GameObject thingThatGotHit)
    {
        //HURT IT!
        if (thingThatGotHit.GetComponent<Health>() != null)
        {
            Health health = thingThatGotHit.GetComponent<Health>();
            float damage = GetLuckDamage(GetAttackTypeDamageNumber());
            health.AdjustHp(-(int)damage, gameObject);
            if (-damage < 0)
            {
                SFXPlayer.I.Play(attackData.playSFXOnHit);
            }
        }
    }

    public virtual void CleanupThatsOnlyCalledFromStateMachine()
    {
        Debug.LogWarning($"{attackData.title} CleanupThatsOnlyCalledFromStateMachine");
        StopTraveling();
        OnCompletedTravel = null;
        RestoreDefaultAppearance();
        SetDefaultPosition();
        gameObject.SetActive(false);
    }
    
    private void CleanupAndSetDefaultState()
    {
        Debug.LogWarning($"{attackData.title} CleanupAndSetDefaultState");
        CleanupThatsOnlyCalledFromStateMachine();
        player.SetState(new StateDefault(player));
    }

    protected float GetLuckDamage(float baseDamage)
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