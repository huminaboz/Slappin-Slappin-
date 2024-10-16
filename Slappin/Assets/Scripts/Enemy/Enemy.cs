using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Health))]
public abstract class Enemy : MonoBehaviour, IHpAdjustmentListener, IObjectPool<Enemy>
{
    private IHpAdjustmentListener _hpAdjustmentListenerImplementation;
    public Health thisHealth { get; set; }

    [SerializeField] private int currency1DropAmount;
    [SerializeField] private GameObject pickupToDrop;
    [SerializeField] private EnemyAnimations _enemyAnimations;
    [SerializeField] private float attackSpeedMultiplier = 1f;

    private MoveTowardsTransform moveTowardsTransform;

    //Flash stuff
    public float flashDuration = 0.5f;
    private Color originalColor;
    private Material thisMaterial;
    public int flashCount = 5;


    protected delegate void EnemyBehavior();

    protected EnemyBehavior performBehavior;

    public virtual void SetupObjectFirstTime()
    {
        gameObject.SetActive(false);
        thisHealth = GetComponent<Health>();

        if (GetComponent<Renderer>())
        {
            thisMaterial = GetComponent<Renderer>().material;
            originalColor = thisMaterial.color;
        }

        moveTowardsTransform = GetComponent<MoveTowardsTransform>();
    }

    public virtual void InitializeObjectFromPool()
    {
        thisHealth.Initialize();
        gameObject.SetActive(true);
        _enemyAnimations?.Play(EnemyAnimations.AnimationFrames.WalkFWD);
    }

    private void Update()
    {
        if (performBehavior is not null) performBehavior();
    }

    public void SwitchToAttackMode()
    {
        performBehavior = Attack;
        _enemyAnimations?.Play(EnemyAnimations.AnimationFrames.Attack01,
            DecideNextAnimation);
    }

    public void TurnOffAttackMode()
    {
        performBehavior -= Attack;
    }

    protected abstract void Attack();

    private void DecideNextAnimation()
    {
        //Called when completing some animations
        if (moveTowardsTransform.IsInAttackRange())
        {
            _enemyAnimations?.Play(EnemyAnimations.AnimationFrames.Attack01,
                DecideNextAnimation);
        }
        else
        {
            _enemyAnimations?.Play(moveTowardsTransform.isDashing
                ? EnemyAnimations.AnimationFrames.RunFWD
                : EnemyAnimations.AnimationFrames.WalkFWD);
        }
    }

    public void TookDamage(int damageAmount, GameObject attacker)
    {
        if (!thisHealth.isAlive) return;
        //Got hit feedback
        if(thisMaterial) StartCoroutine(FlashRedCoroutine());
        VFXSpawner.I.SpawnDamageNumber(damageAmount, transform.position);
        VFXSpawner.I.SpawnHitFX(transform);
        performBehavior = null;
        _enemyAnimations?.Play(EnemyAnimations.AnimationFrames.GetHit,
            DecideNextAnimation);
    }

    private IEnumerator FlashRedCoroutine()
    {
        float flashInterval = flashDuration / (flashCount * 2); // Time for one flash cycle (red to original color)
        
        for (int i = 0; i < flashCount; i++)
        {
            thisMaterial.color = Color.red;
            yield return new WaitForSeconds(flashInterval); // Wait for half of the interval
            thisMaterial.color = originalColor;
            yield return new WaitForSeconds(flashInterval); // Wait for the other half of the interval
        }

        // Ensure the material is set back to its original color
        thisMaterial.color = originalColor;
    }

    public void Healed(int healAmount, GameObject healer)
    {
        //Got healed feedback
    }

    public virtual float HandleDeath(int lastAttack, GameObject killer)
    {
        //TODO: Throw up a puff of particle
        //TODO:: Pop currency(s) out of enemy in a celebration
        Vector3 pickupSpawnPosition = new Vector3(transform.position.x,
            transform.position.y + .02f, transform.position.z);
        Pickup pickup = ObjectPoolManager<Pickup>.GetObject(pickupToDrop);
        pickup.SetupCurrency(currency1DropAmount);
        pickup.SetNewPosition(pickupSpawnPosition);

        SFXPlayer.I.Play(AudioEventsStorage.I.enemyDied);

        //Spin in a circle first
        transform.DORotate(new Vector3(0, 720, 0), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .OnComplete(ReturnObjectToPool);

        performBehavior = null;

        return 0;
    }

    //Abstract because you don't want to return the object to the pool as an Enemy, but as the specific enemy
    public abstract void ReturnObjectToPool();
}