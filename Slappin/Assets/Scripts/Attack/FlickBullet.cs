using UnityEngine;

public class FlickBullet : MonoBehaviour, IObjectPool<FlickBullet>
{
    [SerializeField] private float speed = 400f;

    [HideInInspector] public FlickAttack flickAttack; //Will have to set this up on spawn
    [HideInInspector] public Vector3 spawnPosition;

    [HideInInspector] public float maxTravelDistance;

    [SerializeField] private ParticleSystem _particleSystem;

    private Rigidbody _rigidbody;

    public void SetupObjectFirstTime()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void InitializeObjectFromPool()
    {
        _particleSystem.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        flickAttack.HitSomething(other.gameObject);
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = transform.forward * (Time.fixedDeltaTime * speed);
        float distanceTraveled = Vector3.Distance(spawnPosition, transform.position);
        // Debug.LogWarning($"Distance Traveled {distanceTraveled}\n" +
        //                  $"Max Travel Distance {maxTravelDistance}");
        if (distanceTraveled >= maxTravelDistance)
        {
            //TODO:: Spawn some sort of poof particle effect
            ReturnObjectToPool();
        }
    }

    public void ReturnObjectToPool()
    {
        _particleSystem.Stop();
        ObjectPoolManager<FlickBullet>.ReturnObject(this);
    }
}