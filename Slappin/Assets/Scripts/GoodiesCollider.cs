using UnityEngine;

public class GoodiesCollider : MonoBehaviour
{
    [SerializeField] private AttackType _attackType;
    
    //TODO:: Calculate the attack type's stat for multiplying goodies and etc

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent(typeof(AttackType)) != null)
        {
            Pickup pickup = other.GetComponent<Pickup>();
            _attackType.AddHitPickup(pickup);
            // _attackType.HitSomething(other.gameObject);
        }
    }

}
