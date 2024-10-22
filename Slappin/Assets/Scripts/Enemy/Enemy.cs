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

    [FormerlySerializedAs("currency1DropAmount")] [SerializeField]
    private int currency1BaseDropAmount;

    [SerializeField] private GameObject pickupToDrop;
    [SerializeField] private GameObject healthPickupPrefab;

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

    private void GetFartedOn(float fartDamage, float knockbackForce)
    {
        //Knockback if far enough forward
        if (transform.position.z < EnemyTarget.I.fartLine.position.z)
        {
            Vector3 flickForceVector = new Vector3(0f, 0f, knockbackForce);
            _rigidbody.AddForce(flickForceVector, ForceMode.Impulse);
        }

        thisHealth.AdjustHp((int)-fartDamage, null);
    }

    protected void SetupStats()
    {
        float currencyMultiplier = StatLiason.I.GetEnemy(Stat.Enemy_Currency);
        float hpMultiplier = StatLiason.I.GetEnemy(Stat.Enemy_MaxHp);
        float damageMultiplier = StatLiason.I.GetEnemy(Stat.Enemy_DamageMultiplier);
        float walkSpeedMultiplier = StatLiason.I.GetEnemy(Stat.Enemy_WalkSpeed);

        //Go into the upgrades, send the current wave and set the stats
        currency1DropAmount = currency1BaseDropAmount * currencyMultiplier
                                                      * StatLiason.I.Get(Stat.Currency1Multiplier);
        thisHealth.enemyMaxHp = (int)(thisHealth.maxHp * hpMultiplier);
        damage = baseAttackDamage * damageMultiplier;
        walkSpeed = moveTowardsTransform.baseWalkSpeed * walkSpeedMultiplier;
        moveTowardsTransform.walkSpeed = walkSpeed;

        Debug.Log($"{gameObject.name} - Currency: {currency1DropAmount}. MaxHp: {thisHealth.enemyMaxHp}" +
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

        //NOTE:: This is shitty, but you gotta set this to false at the end of their animation
        isTryingToAttack = true;
    }

    public void TurnOffAttackMode()
    {
        performBehavior -= Attack;
    }

    protected abstract void Attack();

    protected bool isTryingToAttack = false;

    protected void DecideNextAnimation()
    {
        // //Called when completing some animations
        if (!thisHealth.isAlive) return;

        if (moveTowardsTransform.IsInAttackRange() && isTryingToAttack)
        {
            SwitchToAttackMode();
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
        isTryingToAttack = false;
        FartAttack.OnFart -= GetFartedOn;
        //TODO: Throw up a puff of particle
        //TODO:: Pop currency(s) out of enemy in a celebration
        Vector3 pickupSpawnPosition = new Vector3(transform.position.x,
            transform.position.y + .02f, transform.position.z);
        Pickup pickup = ObjectPoolManager<Pickup>.GetObject(pickupToDrop);
        pickup.SetupCurrency((int)currency1DropAmount);
        pickup.SetNewHoverPosition(pickupSpawnPosition);

        if (BozUtilities.GetDiceRoll() < .02f)
        {
            Pickup healthPickup = ObjectPoolManager<Pickup>.GetObject(healthPickupPrefab);
            healthPickup.SetNewHoverPosition(pickupSpawnPosition);
        }

        SFXPlayer.I.Play(AudioEventsStorage.I.enemyDied);
        if (_rigidbody) _rigidbody.velocity = Vector3.zero;

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