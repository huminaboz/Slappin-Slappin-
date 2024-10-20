using System;
using System.Collections;
using UnityEngine;

public class FlickBullet : MonoBehaviour, IObjectPool<FlickBullet>
{
    [SerializeField] private float speed = 400f;

    [HideInInspector] public FlickAttack flickAttack; //Will have to set this up on spawn
    [HideInInspector] public Vector3 spawnPosition;

    [HideInInspector] public float maxTravelDistance;

    [SerializeField] private ParticleSystem _particleSystem;

    private Rigidbody _rigidbody;


    [Header("Flick Impact stuff")] 
    [SerializeField] private bool isFlickImpact = false;
    [SerializeField] private float flickForce = 1f;

    public void SetupObjectFirstTime()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void InitializeObjectFromPool()
    {
        _particleSystem.Play();
    }

    private void OnEnable()
    {
        if (isFlickImpact) StartCoroutine(EndFlickImpact());
    }

    private IEnumerator EndFlickImpact()
    {
        yield return new WaitForSeconds(.1f);
        ReturnObjectToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        flickAttack.HitSomething(other.gameObject);

        if (!isFlickImpact) return;
        if (other.GetComponent<Enemy_Bouncer>()) return;
        if (other.GetComponent<Health>() != null)
        {
            if (other.GetComponent<Health>().isAlive == false) return;
            Vector3 flickForceVector = new Vector3(0f, 0f, flickForce);
            other.GetComponent<Rigidbody>().AddForce(flickForceVector, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        // if (isFlickImpact) return;
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