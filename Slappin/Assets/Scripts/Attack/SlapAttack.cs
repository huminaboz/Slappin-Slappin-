using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SlapAttack : AttackType
{
    // [SerializeField] private Transform shadow;
    [Header("Slap Attack Specific Stuff")]
    
    [SerializeField] private GetHurtOnAttackCollider spikeGetHurtOnAttackCollider;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, 
            offScreenSlapYPosition, transform.position.z);
    }

    public override void InitiateAttack()
    {
        base.InitiateAttack();
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

    private void InitiateTravelToGround()
    {
        goalPosition = new(transform.position.x, groundYPosition, transform.position.z);
        OnCompletedTravel = InitiateTravelBackUp;
        attackSpeed = attackData.attackSpeed;
        startingDistanceFromGoal = Mathf.Abs(handPositioner.position.y - goalPosition.y);
        
        //Giving Direction a value starts up the Fixedupdate telling the hand which way to go
        direction = new(0, -1, 0);
        OnCompletedTravel += () =>
        {
                CameraShake.I.StartCameraShake();
                SFXPlayer.I.Play(AudioEventsStorage.I.slapHitGround);
        };
    }

    private void InitiateTravelBackUp()
    {
        goalPosition = new(transform.position.x, offScreenSlapYPosition, transform.position.z);
        startingDistanceFromGoal = Mathf.Abs(handPositioner.position.y - goalPosition.y);
        OnCompletedTravel = Cleanup;
        attackSpeed = attackData.slapGoUpSpeed;

        //Stop for a bit to see the hand
        const float handRestDuration = .15f;
        direction = Vector3.zero;
        handRigidbody.velocity = Vector3.zero;
        StartCoroutine(BozUtilities.DoAfterDelay(handRestDuration, () =>
        {
            player.EnableMovement();
            direction = new(0, 1, 0);
        }));
    }

    private void FixedUpdate()
    {
        if (direction == Vector3.zero) return;
        float YDistance = Mathf.Abs(handPositioner.position.y - goalPosition.y);
        float ratio = 1f - movementCurve.Evaluate(YDistance / startingDistanceFromGoal);
        ratio = Mathf.Clamp(ratio, .05f, 1f); //Don't let it be 0
        
        handRigidbody.velocity = direction * (Time.fixedDeltaTime * attackSpeed * ratio);

        //Made it to the goal
        if (YDistance <= distanceFlexRoom)
        {
            OnCompletedTravel?.Invoke();
        }
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


}