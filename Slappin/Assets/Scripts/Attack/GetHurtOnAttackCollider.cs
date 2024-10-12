using QFSW.QC;
using UnityEngine;

public class GetHurtOnAttackCollider : MonoBehaviour
{
    [SerializeField] private SlapAttack _slapAttack;
    private Collider _collider;
    
    [SerializeField] private LayerMask _layerMask;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public bool WillHitASpike()
    {
        bool hitASpike = Physics.CapsuleCast(transform.position, transform.position,
            transform.localScale.z * .5f, -transform.up,
            out RaycastHit hit, 10, _layerMask); 
            // if(hit) Debug.Log($"Hit: {hit.collider.gameObject.name}");
            return hitASpike;
    }


    private void OnTriggerEnter(Collider other)
    {
        // if (other.GetComponent(typeof(Enemy_Spike)) != null) 
        // {
        _slapAttack.HitSpike(other.gameObject);
        // }
    }
}