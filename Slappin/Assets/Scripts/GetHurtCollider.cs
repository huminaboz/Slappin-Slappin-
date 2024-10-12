using System;
using UnityEngine;

public class GetHurtCollider : MonoBehaviour
{
    [SerializeField] private SlapAttack _slapAttack;

    private void OnTriggerEnter(Collider other)
    {
        // if (other.GetComponent(typeof(Enemy_Spike)) != null) 
        // {
            _slapAttack.HitSpike(other.gameObject);
        // }
    }
}
