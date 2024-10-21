using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GetHurtOnAttackCollider : MonoBehaviour
{
    [FormerlySerializedAs("_slapAttack")] [SerializeField] private AttackType _thisAttack;
    [SerializeField] private LayerMask _layerMask;

    private Collider _collider;
    
    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public bool WillHitASpike()
    {
        _collider.enabled = true;
        bool hitASpike = Physics.CapsuleCast(transform.position, transform.position,
            transform.localScale.z * .5f, -transform.up,
            out RaycastHit hit, 10, _layerMask); 
            if(hit.collider) Debug.LogWarning($"Hit a spike: {hit.collider.gameObject.name}");
            return hitASpike;
    }

    private void OnTriggerEnter(Collider other)
    {
        _thisAttack.HitSpike(other.gameObject);
        _collider.enabled = false;
    }
}