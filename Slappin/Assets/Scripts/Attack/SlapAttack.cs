using System.Collections;
using UnityEngine;

public class SlapAttack : AttackType
{
    // [SerializeField] private Transform shadow;
    [Header("Slap Attack Specific Stuff")] [SerializeField]
    protected GameObject pickupColliderObject;

    [SerializeField] private GetHurtOnAttackCollider spikeGetHurtOnAttackCollider;
    [SerializeField] private GameObject handModel;
    [SerializeField] private Transform slapForecastShadow;
    
    private Vector3 _hurtColliderDefaultLocalScale;
    private Vector3 _slapForecastShadowDefaultLocalScale;
    private float distanceDamageBoost;


    private Vector3 _attackColliderDefaultLocalPosition;
    
    //Animation
    [SerializeField] private Animator animator;
    
    private void OnEnable()
    {
        UpgradeData.OnPurchaseMade += UpdateColliderAndForecastSize;
    }

    private void OnDisable()
    {
        UpgradeData.OnPurchaseMade -= UpdateColliderAndForecastSize;
    }

    public override void Initialize()
    {
        base.Initialize();
        handModel?.SetActive(false);
        _hurtColliderDefaultLocalScale = hurtEnemiesColliderObject.transform.localScale;
        _slapForecastShadowDefaultLocalScale = slapForecastShadow.transform.localScale;
        _attackColliderDefaultLocalPosition = hurtEnemiesColliderObject.transform.localPosition;
    }

    protected override float GetAttackTypeDamageNumber()
    {
        float damage = StatLiason.I.Get(Stat.SlapDamage);
        // Debug.LogWarning($"Slap damage was {damage} before distance damage boost");
        damage +=  Mathf.Ceil(damage * distanceDamageBoost);
        // Debug.LogWarning($"AND NOW Slap damage is {damage} after distance damage boost");
        return damage;
    }


    
    public override void InitiateAttack()
    {
        base.InitiateAttack();
        handModel?.SetActive(true);

        //Set it upon attack since that's where the distance comes from
        distanceDamageBoost = GetRangedDamageBonus(StatLiason.I.Get(Stat.SlapDamagePerDistance));
        hurtEnemiesColliderObject.transform.localPosition = new(100, 100, 100);
        PlayAnimationCoroutine(.1f, "Down");
        DropSlap();
    }

    private Coroutine _slapAnimationCoroutine;

    private void PlayAnimationCoroutine(float delay, string animationName)
    {
        if(_slapAnimationCoroutine != null) StopCoroutine(_slapAnimationCoroutine);
        _slapAnimationCoroutine = StartCoroutine(PlayAnimation(delay, animationName));
    }
    
    private IEnumerator PlayAnimation(float delay, string animationName)
    {
        yield return new WaitForSeconds(delay);
        animator.Play(animationName);
    }

    private void UpdateColliderAndForecastSize()
    {
        //Set the collider size based on the slap size when you call a slap
        hurtEnemiesColliderObject.transform.localScale = 
            new Vector3(_hurtColliderDefaultLocalScale.x * StatLiason.I.Get(Stat.SlapAreaMultiplier),
                _hurtColliderDefaultLocalScale.y,
                _hurtColliderDefaultLocalScale.z * StatLiason.I.Get(Stat.SlapAreaMultiplier)); 
        
        //And the forecast size
        slapForecastShadow.localScale = 
            new Vector3(_slapForecastShadowDefaultLocalScale.x * StatLiason.I.Get(Stat.SlapAreaMultiplier),
                _slapForecastShadowDefaultLocalScale.y,
                _slapForecastShadowDefaultLocalScale.z * StatLiason.I.Get(Stat.SlapAreaMultiplier)); 
    }

    private void DropSlap()
    {
        //TODO:: Consider moving this up to the parent - decide while making other attacks
        pickupColliderObject.SetActive(true);
        hurtEnemiesColliderObject.SetActive(true);

        //TODO:: there might be more things that can hurt -
        //or can genericize it on the parent for other hand types that can get hurt
        spikeGetHurtOnAttackCollider.gameObject.SetActive(true);

        if (spikeGetHurtOnAttackCollider.WillHitASpike())
        {
            pickupColliderObject.SetActive(false);
            hurtEnemiesColliderObject.SetActive(false);
        }
        else
        {
            spikeGetHurtOnAttackCollider.gameObject.SetActive(false);
        }

        InitiateTravelToGround();
    }

    protected override void InitiateTravelToGround()
    {
        OnCompletedTravel = InitiateTravelBackUp;
        base.InitiateTravelToGround();
    }

    protected override void DoWhenReachingGround()
    {
        base.DoWhenReachingGround();
        //PlayAnimationCoroutine(0f, "Pause");
        CameraShake.I.StartCameraShake();
        SFXPlayer.I.Play(AudioEventsStorage.I.slapHitGround);
        hurtEnemiesColliderObject.transform.localPosition = _attackColliderDefaultLocalPosition;
    }

    private Coroutine stunDelayCoroutine;
    public override void InitiateTravelBackUp()
    {
        //Stop for a bit to see the hand
            PlayAnimationCoroutine(.1f, "Up");
        const float handRestDuration = .15f;
        if(stunDelayCoroutine != null) StopCoroutine(stunDelayCoroutine);
        stunDelayCoroutine = StartCoroutine(BozUtilities.DoAfterDelay(handRestDuration, () =>
        {
            player.EnableMovement();
            base.InitiateTravelBackUp();
        }));
    }

    public override void HitSpike(GameObject spike)
    {
        //If hitting a spike, take damage and go back up
        spikeGetHurtOnAttackCollider.gameObject.SetActive(false); //Don't accidentally hit another
        Direction = Vector3.zero;
        handRigidbody.velocity = Vector3.zero;

        int handDamage = 0;
        float handStabStunDuration = .15f;
        //TODO:: Make an interface for enemies with spikes
        if (spike.GetComponent<Enemy_Spike>())
        {
            Enemy_Spike enemySpike = spike.GetComponent<Enemy_Spike>();
            handDamage = (int) enemySpike.damage;
            handStabStunDuration = enemySpike.handStabStunDuration;
        }

        if (spike.GetComponent<Enemy_Turtle>())
        {
            Enemy_Turtle enemyTurtle = spike.GetComponent<Enemy_Turtle>();
            handDamage = enemyTurtle.handStabDamage;
        }
        
        playerHealth.AdjustHp(-handDamage, gameObject);
        player.SetState(new StateDamagedState(player));

        StartCoroutine(BozUtilities.DoAfterDelay(handStabStunDuration
                                                 * PlayerStats.I.stunRecoveryMultiplier,
            InitiateTravelBackUp));
    }

    public override void Cleanup()
    {
        handModel?.SetActive(false);

        base.Cleanup();
    }
}