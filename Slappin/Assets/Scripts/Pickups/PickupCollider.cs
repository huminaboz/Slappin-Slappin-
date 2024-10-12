using UnityEngine;

public class PickupCollider : MonoBehaviour, IPickerUpper
{
    // [SerializeField] private AttackType _attackType;
    
    //TODO:: Calculate the attack type's stat for multiplying goodies and etc

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.GetComponent(typeof(Pickup)) != null)
    //     {
    //         Pickup pickup = other.GetComponent<Pickup>();
    //         _attackType.HitSomething(other.gameObject);
    //     }
    // }

}
