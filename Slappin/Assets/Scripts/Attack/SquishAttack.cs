using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquishAttack : AttackType
{
    [Header("Squish Specific Stuff")] [SerializeField]
    private GetHurtOnAttackCollider spikeGetHurtOnAttackCollider;

    [SerializeField] private GameObject handModel;
    [SerializeField] private Transform slapForecastShadow;

    private Vector3 _hurtColliderDefaultLocalScale;
    private Vector3 _slapForecastShadowDefaultLocalScale;
    private float distanceDamageBoost;

    private bool touchingGround = false;

    private Action _currentAction;
    private ObjectShake _shake;

    public override void Initialize()
    {
        base.Initialize();
        handModel?.SetActive(false);
        _hurtColliderDefaultLocalScale = hurtEnemiesColliderObject.transform.localScale;
        _slapForecastShadowDefaultLocalScale = slapForecastShadow.transform.localScale;
        _shake = GetComponent<ObjectShake>();
    }

    protected override float GetAttackTypeDamageNumber()
    {
        float damage = StatLiason.I.Get(Stat.Squish);
        Debug.LogWarning($"Squish damage was {damage} before distance damage boost");
        damage += Mathf.Ceil(damage * distanceDamageBoost);
        Debug.LogWarning($"AND NOW Squish damage is {damage} after distance damage boost");
        return damage;
    }

    public override void InitiateAttack()
    {
        base.InitiateAttack();
        handModel?.SetActive(true);

        //Set it upon attack since that's where the distance comes from
        distanceDamageBoost = GetRangedDamageBonus(StatLiason.I.Get(Stat.SlapDamagePerDistance));

        //TODO::I dunno, try to include drop speed into the animation delay
        float dropSpeed = attackData.goDownSpeed;

        DropSquish();
    }

    private void DropSquish()
    {
        hurtEnemiesColliderObject.SetActive(true);

        spikeGetHurtOnAttackCollider.gameObject.SetActive(true);

        if (spikeGetHurtOnAttackCollider.WillHitASpike())
        {
            hurtEnemiesColliderObject.SetActive(false);
        }
        else
        {
            spikeGetHurtOnAttackCollider.gameObject.SetActive(false);
        }

        InitiateTravelToGround();
    }

    protected override void DoWhenReachingGround()
    {
        base.DoWhenReachingGround();
        //TODO:: Start playing some enemy hitting stuff
        SFXPlayer.I.Play(AudioEventsStorage.I.squishHitGround);
        touchingGround = true;
        _shake.StartShake();
        _currentAction = DoShitWhileTouchingGround;
    }

    private void Update()
    {
        _currentAction?.Invoke();
    }

    //For calling on the playerstate
    private void DoShitWhileTouchingGround()
    {
        Debug.Log("Touching the ground, hurt things!");

        if (!Input.GetButton("Fire4"))
        {
            _currentAction = null;
            InitiateTravelBackUp();
        }
    }

    public override void InitiateTravelBackUp()
    {
        //TODO:: Once you release the button
        _shake.StopShake();

        base.InitiateTravelBackUp();
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
            handDamage = (int)enemySpike.damage;
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