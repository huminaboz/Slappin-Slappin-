using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class SlapAttack : AttackType, IHpAdjustmentListener
{
    // [SerializeField] private Transform shadow;
    [SerializeField] private Transform slapPosition;
    [SerializeField] private Health playerHealth;
    [SerializeField] private GameObject handModel;
    [SerializeField] private Player player;

    //Todo:: Just make this disappear
    [SerializeField] private float offScreenSlapYPosition = -0.23f; //Measured by holding it off camera
    [SerializeField] private float groundYPosition = -0.78f; //Measured by putting the hand on the ground 

    [SerializeField] private AnimationCurve startSlapCurve;

    [SerializeField] private Renderer slapRenderer;
    private Material slapMaterial;
    private Color defaultBottomOfHandColor;

    private Rigidbody _slapRigidbody;
    private Vector3 direction;
    private Vector3 goalPosition;
    private float attackSpeed;
    private const float distanceFlexRoom = .05f;

    private Action OnCompletedTravel;
    [SerializeField] private GetHurtOnAttackCollider spikeGetHurtOnAttackCollider;
    [SerializeField] private GameObject pickupColliderObject;
    [SerializeField] private GameObject hitEnemiesColliderObject;

    private void Awake()
    {
        slapMaterial = slapRenderer.material;
        defaultBottomOfHandColor = slapRenderer.material.GetColor("_ColorDimExtra");
        _slapRigidbody = slapPosition.GetComponent<Rigidbody>();
        handModel.SetActive(false);
    }

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, offScreenSlapYPosition, transform.position.z);
    }

    private Tween downTween;

    public void DropSlap()
    {
        if (!playerHealth.isAlive)
        {
            Debug.LogWarning("Can't slap - player is dead");
            return;
        }
        
        handModel.SetActive(true);
        pickupColliderObject.SetActive(true);
        hitEnemiesColliderObject.SetActive(true);
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
        player.DisableInputs();
        goalPosition = new(transform.position.x, groundYPosition, transform.position.z);
        //TODO:: Put in a delay before heading back up
        OnCompletedTravel = HeadBackUp;
        attackSpeed = attackData.attackSpeed;
        
        
        //Give Direction a value starts up the Fixedupdate telling the hand which way to go
        direction = new(0, -1, 0);
    }

    private void HeadBackUp()
    {
        player.EnableMovement();
        goalPosition = new(transform.position.x, offScreenSlapYPosition, transform.position.z);
        OnCompletedTravel = StopMoving;
        attackSpeed = attackData.slapGoUpSpeed;
        
        direction = new(0, 1, 0);
    }

    private void StopMoving()
    {
        _slapRigidbody.velocity = Vector3.zero;
        direction = Vector3.zero;
        slapRenderer.material.SetColor("_ColorDimExtra", defaultBottomOfHandColor);
        player.EnablePlayerAttacks();
        handModel.SetActive(false);
    }

    private void FixedUpdate()
    {
        float YDistance = Mathf.Abs(slapPosition.position.y - goalPosition.y);
        if (direction == Vector3.zero) return;
        //TODO:: Map acceleration along a animation curve - can use attackSpeed as the goal value
        _slapRigidbody.velocity = direction * (Time.fixedDeltaTime * attackSpeed);

        //Made it to the goal
        if (YDistance <= distanceFlexRoom)
        {
            //TODO:: Put in a delay
            OnCompletedTravel?.Invoke();
        }
    }

    public void HitSpike(GameObject thingThatGotHit)
    {
        //If hitting a spike, take damage and go back up
        direction = Vector3.zero;
        _slapRigidbody.velocity = Vector3.zero;
        Enemy_Spike enemySpike = thingThatGotHit.GetComponent<Enemy_Spike>();
        playerHealth.AdjustHp(-enemySpike.handStabDamage, gameObject);
        slapMaterial.SetColor("_ColorDimExtra", Color.red);
        //TODO:: Make a timer to get stunned, then when done set the direction to go back up
        StartCoroutine(DoAfterDelay(attackData.slapRecoverFromSpikeTimer, HeadBackUp));
    }

    private IEnumerator DoAfterDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    public void TookDamage(int damageAmount, GameObject attacker)
    {
        //Feedback on the hand that it hit a spike
    }

    public void Healed(int healAmount, GameObject healer)
    {
    }

    public float HandleDeath(int lastAttack, GameObject killer)
    {
        downTween.Kill();
        return 0;
    }
}