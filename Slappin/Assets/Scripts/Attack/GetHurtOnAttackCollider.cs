using UnityEngine;

public class GetHurtOnAttackCollider : MonoBehaviour
{
    [SerializeField] private SlapAttack _slapAttack;
    [SerializeField] private LayerMask _layerMask;

    public bool WillHitASpike()
    {
        bool hitASpike = Physics.CapsuleCast(transform.position, transform.position,
            transform.localScale.z * .5f, -transform.up,
            out RaycastHit hit, 10, _layerMask); 
            if(hit.collider) Debug.LogWarning($"Hit: {hit.collider.gameObject.name}");
            return hitASpike;
    }

    private void OnTriggerEnter(Collider other)
    {
        _slapAttack.HitSpike(other.gameObject);
    }
}