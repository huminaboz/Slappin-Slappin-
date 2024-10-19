using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SlapAttack : AttackType
{
    // [SerializeField] private Transform shadow;
    [Header("Slap Attack Specific Stuff")] [SerializeField]
    protected GameObject pickupColliderObject;

    [SerializeField] private GetHurtOnAttackCollider spikeGetHurtOnAttackCollider;
    [SerializeField] private GameObject handModel;
    [SerializeField] private Transform slapForecastShadow;
    
    //TODO:: And the pickup collider drawing from an upgrade area too
    private Vector3 _hurtColliderDefaultLocalScale;
    private Vector3 _slapForecastShadowDefaultLocalScale;


    private void OnEnable()
    {
        UpgradeData.OnPurchaseMade += UpdateColliderAndForecastSize;
    }

    private void OnDisable()
    {
        //I THINK this should be enabled while you're in the store???
        UpgradeData.OnPurchaseMade -= UpdateColliderAndForecastSize;
    }

    public override void Initialize()
    {
        base.Initialize();
        handModel?.SetActive(false);
        _hurtColliderDefaultLocalScale = hurtEnemiesColliderObject.transform.localScale;
        _slapForecastShadowDefaultLocalScale = slapForecastShadow.transform.localScale;
    }

    protected override float GetAttackTypeDamageNumber()
    {
        float damage = StatLiason.I.Get(Stat.SlapDamage);
        return damage;
    }

    public override void InitiateAttack()
    {
        base.InitiateAttack();
        handModel?.SetActive(true);

        DropSlap();
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
        CameraShake.I.StartCameraShake();
        SFXPlayer.I.Play(AudioEventsStorage.I.slapHitGround);
    }

    protected override void InitiateTravelBackUp()
    {
        //Stop for a bit to see the hand
        const float handRestDuration = .15f;
        StartCoroutine(BozUtilities.DoAfterDelay(handRestDuration, () =>
        {
            player.EnableMovement();
            base.InitiateTravelBackUp();
        }));
    }

    public void HitSpike(GameObject spike)
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
            handDamage = enemySpike.handStabDamage;
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

    protected override void Cleanup()
    {
        handModel?.SetActive(false);

        base.Cleanup();
    }
}