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

    [FormerlySerializedAs("currencyDropAmount")] [FormerlySerializedAs("currencyDrop")] [SerializeField] private int currency1DropAmount;
    [SerializeField] private GameObject pickupToDrop;
    
    
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
        thisMaterial = GetComponent<Renderer>().material;
        originalColor = thisMaterial.color;
    }

    public virtual void InitializeObjectFromPool()
    {
        thisHealth.Initialize();
        gameObject.SetActive(true);
    }

    private void Update()
    {
        //TODO:: Do attack
        if(performBehavior is not null) performBehavior();
    }

    public void SwitchToAttackMode()
    {
        performBehavior = Attack;
    }

    public void TurnOffAttackMode()
    {
        performBehavior -= Attack;
    }
    protected abstract void Attack();

    public void TookDamage(int damageAmount, GameObject attacker)
    {
        //Got hit feedback
        StartCoroutine(FlashRedCoroutine());
        VFXSpawner.I.SpawnDamageNumber(damageAmount, transform.position);
        VFXSpawner.I.SpawnHitFX(transform);
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