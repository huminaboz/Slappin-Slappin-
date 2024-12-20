using UnityEngine;
using UnityEngine.Serialization;

public class EnemyProjectile : MonoBehaviour, IObjectPool<EnemyProjectile>, IHpAdjustmentListener
{
    //Stats
    public float damage;

    //Movement
    private Rigidbody _rigidbody;
    [SerializeField] private float speed;
    private Vector3 goalPosition;

    [FormerlySerializedAs("pickupZoneZOffset")] [SerializeField]
    private float hitPlayerZoneZOffset = 1f;


    Health health;

    public void SetupObjectFirstTime()
    {
        health = GetComponent<Health>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void InitializeObjectFromPool()
    {
        transform.position = new Vector3(transform.position.x,
            transform.position.y , transform.position.z);
        goalPosition = new Vector3(transform.position.x, transform.position.y,
            Camera.main.transform.position.z);
        health.enabled = true;
        health.Initialize();
        health.hp = 1;
        FartAttack.OnFart += GetFartedOn;
        GameplayUIManager.StartedNewWave += NewWavePushback;
    }
    
    private void GetFartedOn(float fartDamage, float knockback)
    {
        health.AdjustHp((int)-fartDamage, null);
    }

    private void FixedUpdate()
    {
        Vector3 direction = goalPosition - transform.position;
        direction.y = 0f; //Manually set how high up it goes

        // Move the object towards the target
        _rigidbody.velocity = direction.normalized * speed * Time.deltaTime;

        if (transform.localPosition.z <= goalPosition.z + hitPlayerZoneZOffset)
        {
            SFXPlayer.I.Play(AudioEventsStorage.I.enemyAttacked);
            PlayerInfo.I.health.AdjustHp((int)damage, gameObject);
            ReturnObjectToPool();
        }
    }
    
    private void NewWavePushback()
    {
        //Knockback if far enough forward
        if (transform.position.z < EnemyTarget.I.fartLine.position.z)
        {
            Vector3 flickForceVector = new Vector3(0f, 0f, 40f);
            _rigidbody.AddForce(flickForceVector, ForceMode.Impulse);
        }

        // thisHealth.AdjustHp((int)-10, null);
    }


    public void ReturnObjectToPool()
    {
        ObjectPoolManager<EnemyProjectile>.ReturnObject(this);
    }

    public void TookDamage(int damageAmount, GameObject attacker)
    {
    }

    public void Healed(int healAmount, GameObject healer)
    {
    }
    

    public float HandleDeath(int lastAttack, GameObject killer)
    {
        SFXPlayer.I.Play(AudioEventsStorage.I.snuffedProjectile);
        health.enabled = false;
        FartAttack.OnFart -= GetFartedOn;
        GameplayUIManager.StartedNewWave -= NewWavePushback;
        ReturnObjectToPool();
        return 0;
    }
}