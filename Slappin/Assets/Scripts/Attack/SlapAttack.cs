using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SlapAttack : AttackType
{
    // [SerializeField] private Transform shadow;
    [Header("Slap Attack Specific Stuff")]
    
    [SerializeField] protected GameObject pickupColliderObject;

    [SerializeField] private GetHurtOnAttackCollider spikeGetHurtOnAttackCollider;
    [SerializeField] private GameObject handModel;


    public override void Initialize()
    {
        base.Initialize();
        handModel?.SetActive(false);
    }

    public override void InitiateAttack()
    {
        base.InitiateAttack();
        handModel?.SetActive(true);

        DropSlap();
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
        direction = Vector3.zero;
        handRigidbody.velocity = Vector3.zero;
        Enemy_Spike enemySpike = spike.GetComponent<Enemy_Spike>();
        playerHealth.AdjustHp(-enemySpike.handStabDamage, gameObject);
        player.SetState(new StateDamagedState(player));
        
        StartCoroutine(BozUtilities.DoAfterDelay(enemySpike.handStabStunDuration 
                                    * PlayerStats.I.stunRecoveryMultiplier, 
            InitiateTravelBackUp));
    }

    protected override void Cleanup()
    {
        handModel?.SetActive(false);

        base.Cleanup();
    }
}