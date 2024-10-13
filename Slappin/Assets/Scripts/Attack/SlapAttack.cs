using System;
using UnityEngine;

public class SlapAttack : AttackType
{
    // [SerializeField] private Transform shadow;
    [SerializeField] private Transform slapPosition;
    [SerializeField] private Health playerHealth;
    [SerializeField] private GameObject handModel;
    [SerializeField] private float offScreenSlapYPosition = -0.23f; //Measured by holding it off camera
    [SerializeField] private float groundYPosition = -0.78f; //Measured by putting the hand on the ground 
    [SerializeField] private AnimationCurve startSlapCurve;
    [SerializeField] private GetHurtOnAttackCollider spikeGetHurtOnAttackCollider;
    [SerializeField] private GameObject pickupColliderObject;
    [SerializeField] private GameObject hitEnemiesColliderObject;

    private Vector3 direction;
    private Vector3 goalPosition;
    private float attackSpeed;
    private const float distanceFlexRoom = .05f;
    private float startingDistanceFromGoal;
    private Action OnCompletedTravel;
    

    private void Awake()
    {
        handModel.SetActive(false);
    }

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, 
            offScreenSlapYPosition, transform.position.z);
    }

    public override void DoAttack()
    {
        base.DoAttack();
        DropSlap();
    }

    public void DropSlap()
    {
        if (!playerHealth.isAlive)
        {
            Debug.LogWarning("Can't slap - player is dead");
            return;
        }

        handModel.SetActive(true);
        //TODO:: Consider moving this up to the parent - decide while making other attacks
        pickupColliderObject.SetActive(true);
        hitEnemiesColliderObject.SetActive(true);
        
        //TODO:: there might be more things that can hurt -
        //or can genericize it on the parent for other hand types that can get hurt
        spikeGetHurtOnAttackCollider.gameObject.SetActive(true);

        if (spikeGetHurtOnAttackCollider.WillHitASpike())
        {
            pickupColliderObject.SetActive(false);
            hitEnemiesColliderObject.SetActive(false);
        }
        else
        {
            spikeGetHurtOnAttackCollider.gameObject.SetActive(false);
        }

        HeadToGround();
    }

    private void HeadToGround()
    {
        player.DisableMovement();
        goalPosition = new(transform.position.x, groundYPosition, transform.position.z);
        OnCompletedTravel = HeadBackUp;
        attackSpeed = attackData.attackSpeed;
        startingDistanceFromGoal = Mathf.Abs(slapPosition.position.y - goalPosition.y);
        
        //Giving Direction a value starts up the Fixedupdate telling the hand which way to go
        direction = new(0, -1, 0);
    }

    private void HeadBackUp()
    {
        goalPosition = new(transform.position.x, offScreenSlapYPosition, transform.position.z);
        startingDistanceFromGoal = Mathf.Abs(slapPosition.position.y - goalPosition.y);
        OnCompletedTravel = StopMoving;
        attackSpeed = attackData.slapGoUpSpeed;

        //Stop for a bit to see the hand
        const float handRestDuration = .15f;
        direction = Vector3.zero;
        _handRigidbody.velocity = Vector3.zero;
        StartCoroutine(BozUtilities.DoAfterDelay(handRestDuration, () =>
        {
            player.EnableMovement();
            direction = new(0, 1, 0);
        }));
    }

    //This is that substate stuff that's a bit confusing
    private void StopMoving()
    {
        _handRigidbody.velocity = Vector3.zero;
        direction = Vector3.zero;
        handModel.SetActive(false);
        player.SetState(new StateDefault(player));
    }

    private void FixedUpdate()
    {
        float YDistance = Mathf.Abs(slapPosition.position.y - goalPosition.y);
        if (direction == Vector3.zero) return;
        float ratio = 1f - startSlapCurve.Evaluate(YDistance / startingDistanceFromGoal);
        ratio = Mathf.Clamp(ratio, .05f, 1f); //Don't let it be 0
        
        _handRigidbody.velocity = direction * (Time.fixedDeltaTime * attackSpeed * ratio);

        //Made it to the goal
        if (YDistance <= distanceFlexRoom)
        {
            if (direction == Vector3.down) //When we know it's slapping down
            {
                CameraShake.I.StartCameraShake();
                SFXPlayer.I.Play(AudioEventsStorage.I.slapHitGround);
            }
            OnCompletedTravel?.Invoke();
        }
    }

    public void HitSpike(GameObject spike)
    {
        //If hitting a spike, take damage and go back up
        spikeGetHurtOnAttackCollider.gameObject.SetActive(false); //Don't accidentally hit another
        direction = Vector3.zero;
        _handRigidbody.velocity = Vector3.zero;
        Enemy_Spike enemySpike = spike.GetComponent<Enemy_Spike>();
        playerHealth.AdjustHp(-enemySpike.handStabDamage, gameObject);

        StartCoroutine(BozUtilities.DoAfterDelay(enemySpike.handStabStunDuration 
                                    * PlayerStats.I.stunRecoveryMultiplier, 
            HeadBackUp));
    }


}