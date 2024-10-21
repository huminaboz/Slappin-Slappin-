using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Health))]
public abstract class Enemy : MonoBehaviour, IHpAdjustmentListener, IObjectPool<Enemy>
{
    public Health thisHealth { get; set; }
    
    [SerializeField] protected EnemyAnimations _enemyAnimations;

    [FormerlySerializedAs("currency1DropAmount")] [SerializeField] private int currency1BaseDropAmount;
    [SerializeField] private GameObject pickupToDrop;

    //Movement
    [SerializeField] private Rigidbody _rigidbody;
    
    
    private IHpAdjustmentListener _hpAdjustmentListenerImplementation;
    private MoveTowardsTransform moveTowardsTransform;

    //Flash stuff
    public float flashDuration = 0.5f;
    private Color originalColor;
    private Material thisMaterial;
    public int flashCount = 5;

    //STATS SET BY DIFFICULTY
    [SerializeField] protected int baseAttackDamage = 10;
    [SerializeField] private SO_Upgrade damageIncreaser;
    [FormerlySerializedAs("walkSpeedIncreaser")] [SerializeField] private SO_Upgrade walkSpeedMultiplier;
    [SerializeField] private SO_Upgrade currencyDropAmountIncreaser;
    [SerializeField] private SO_Upgrade maxHpIncreaser;
    [HideInInspector] public float damage;
    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float currency1DropAmount;
    
    
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
        _rigidbody.isKinematic = false;
        SetupStats();
        thisHealth.enabled = true;
        thisHealth.Initialize();
        gameObject.SetActive(true);
        _enemyAnimations?.Play(EnemyAnimations.AnimationFrames.WalkFWD);
        FartAttack.OnFart += GetFartedOn;
    }

    private void GetFartedOn(float fartDamage)
    {
        thisHealth.AdjustHp((int)-fartDamage, null);
    }

    protected void SetupStats()
    {
        int wave = DifficultyManager.I.currentWave;
        //Go into the upgrades, send the current wave and set the stats
        currency1DropAmount = currencyDropAmountIncreaser
            .newValueGrowthCurve.ComputeGrowth(currency1BaseDropAmount, wave);
        thisHealth.enemyMaxHp = (int) maxHpIncreaser.newValueGrowthCurve
            .ComputeGrowth(thisHealth.maxHp, wave);
        damage = damageIncreaser.newValueGrowthCurve.ComputeGrowth(baseAttackDamage, wave);
        walkSpeed = moveTowardsTransform.baseWalkSpeed * 
                    walkSpeedMultiplier.newValueGrowthCurve
                        .ComputeGrowth(walkSpeedMultiplier.baseValue, wave);
        moveTowardsTransform.walkSpeed = walkSpeed;

        Debug.LogWarning($"{gameObject.name} - Currency: {currency1DropAmount}. MaxHp: {thisHealth.enemyMaxHp}" +
                         $"\n Damage: {damage}, WalkSpeed: {walkSpeed}");
    }
    

    private void Update()
    {
        if (!thisHealth.isAlive) return;
        performBehavior?.Invoke();
    }

    public virtual void SwitchToAttackMode()
    {
        performBehavior = Attack;
    }

    public void TurnOffAttackMode()
    {
        performBehavior -= Attack;
    }

    protected abstract void Attack();

    protected void DecideNextAnimation()
    {
        if (!thisHealth.isAlive) return;
        
        // //Called when completing some animations
        // if (moveTowardsTransform.IsInAttackRange())
        // {
        //     _enemyAnimations?.Play(EnemyAnimations.AnimationFrames.Attack01,
        //         DecideNextAnimation);
        // }
        // else
        // {
            _enemyAnimations?.Play(moveTowardsTransform.isDashing
                ? EnemyAnimations.AnimationFrames.RunFWD
                : EnemyAnimations.AnimationFrames.WalkFWD);
        // }
    }

    public void TookDamage(int damageAmount, GameObject attacker)
    {
        if (!thisHealth.isAlive) return;
        //Got hit feedback
        if (thisMaterial) StartCoroutine(FlashRedCoroutine());
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
        FartAttack.OnFart -= GetFartedOn;
        //TODO: Throw up a puff of particle
        //TODO:: Pop currency(s) out of enemy in a celebration
        Vector3 pickupSpawnPosition = new Vector3(transform.position.x,
            transform.position.y + .02f, transform.position.z);
        Pickup pickup = ObjectPoolManager<Pickup>.GetObject(pickupToDrop);
        pickup.SetupCurrency((int)currency1DropAmount);
        pickup.SetNewHoverPosition(pickupSpawnPosition);

        SFXPlayer.I.Play(AudioEventsStorage.I.enemyDied);
        if(_rigidbody) _rigidbody.velocity = Vector3.zero;

        if (_enemyAnimations)
        {
            _enemyAnimations.Play(EnemyAnimations.AnimationFrames.Die, () =>
            {
                //Let the final frame of die sit for a second
                StartCoroutine(BozUtilities.DoAfterDelay(.2f, ReturnObjectToPool));
            });
        }
        else
        {
            //Spin in a circle first
            transform.DORotate(new Vector3(0, 720, 0), 1f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .OnComplete(ReturnObjectToPool);
        }

        performBehavior = null;
        thisHealth.enabled = false;
        _rigidbody.isKinematic = true;

        return 0;
    }

    //Abstract because you don't want to return the object to the pool as an Enemy, but as the specific enemy
    public abstract void ReturnObjectToPool();
}