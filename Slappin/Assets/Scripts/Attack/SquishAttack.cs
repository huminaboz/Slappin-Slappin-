using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquishAttack : AttackType
{
    [Header("Squish Specific Stuff")]

    //Colliders
    [SerializeField]
    private GetHurtOnAttackCollider spikeGetHurtOnAttackCollider;

    [SerializeField] private GameObject damageOverTimeCollider;

    [SerializeField] private GameObject handModel;
    [SerializeField] private Transform slapForecastShadow;

    private float distanceDamageBoost;

    private Action _currentAction;
    private ObjectShake _shake;

    public override void Initialize()
    {
        base.Initialize();
        handModel?.SetActive(false);
        _shake = GetComponent<ObjectShake>();
    }

    protected override float GetAttackTypeDamageNumber()
    {
        float damage = StatLiason.I.Get(Stat.SquishDamage);
        Debug.LogWarning($"Squish damage was {damage} before distance damage boost");
        damage += Mathf.Ceil(damage * distanceDamageBoost);
        Debug.LogWarning($"AND NOW Squish damage is {damage} after distance damage boost");
        return damage;
    }

    public override void InitiateAttack()
    {
        animator.enabled = false;
        //Little extra cleanup in case some async stuff is happening
        StopAllCoroutines(); 
        _currentAction = null;
        
        //The attack as scheduled
        base.InitiateAttack();
        handModel?.SetActive(true);

        //Set it upon attack since that's where the distance comes from
        distanceDamageBoost = GetRangedDamageBonus(StatLiason.I.Get(Stat.SquishDamagePerDistance));

        DropSquish();
        animator.enabled = true;
        PlayAnimationCoroutine(0f, "Down");
    }

    private void DropSquish()
    {
        damageOverTimeCollider.SetActive(false);
        spikeGetHurtOnAttackCollider.gameObject.SetActive(true);

        if (spikeGetHurtOnAttackCollider.WillHitASpike())
        {
            hurtEnemiesColliderObject.SetActive(false);
        }
        else
        {
            hurtEnemiesColliderObject.SetActive(true);
            spikeGetHurtOnAttackCollider.gameObject.SetActive(false);
        }

        InitiateTravelToGround();
    }
    
    protected override void DoWhenReachingGround()
    {
        base.DoWhenReachingGround();
        //TODO:: Start playing some enemy hitting stuff
        SFXPlayer.I.Play(AudioEventsStorage.I.squishHitGround);
        //_shake.StartShake();
        _currentAction = DoShitWhileTouchingGround;
        StartCoroutine(SwapDamageColliders());
        PlayAnimationCoroutine(.1f, "Squishing");
    }

    private IEnumerator SwapDamageColliders()
    {
        yield return new WaitForSeconds(0.1f);
        hurtEnemiesColliderObject.SetActive(false);
        yield return new WaitForSeconds(0.25f);
        damageOverTimeCollider.SetActive(true);
    }

    private void Update()
    {
        _currentAction?.Invoke();
    }
    
    private void DoShitWhileTouchingGround()
    {
        if (player._inputSystem.Player.Squish.IsInProgress()) return;
        Debug.Log("squish button was released");
        _currentAction = null;
        InitiateTravelBackUp();
    }

    public override void InitiateTravelBackUp()
    {
        //Once you release the button
        _shake.StopShake();
        damageOverTimeCollider.SetActive(false);
        
        //PlayAnimationCoroutine(0f, "Up");
        base.InitiateTravelBackUp();
    }

    private Coroutine stunDelayCoroutine;
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

        if(stunDelayCoroutine != null) StopCoroutine(stunDelayCoroutine);
        stunDelayCoroutine = StartCoroutine(BozUtilities.DoAfterDelay(handStabStunDuration
                                                                   * PlayerStats.I.stunRecoveryMultiplier,
            InitiateTravelBackUp));
    }

    public override void Cleanup()
    {
        handModel?.SetActive(false);

        base.Cleanup();
    }
}