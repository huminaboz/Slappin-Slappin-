using UnityEngine;

[RequireComponent(typeof(Health))]
public abstract class Enemy : MonoBehaviour, IDamageable
{
    public Health thisHealth { get; set; }
    
    private void Awake()
    {
        thisHealth = GetComponent<Health>();
    }
    
    public void AdjustHealth(int amount)
    {
        thisHealth.AdjustHp(amount, this);
        Debug.Log($"{gameObject.name} damaged for {amount}. "
                  + $"\nHp is now {thisHealth.Hp}"
        );
    }
    
    public virtual void HandleDeath()
    {
        //Throw up a puff of particle
        //Return to the pool
        
        Destroy(gameObject);
    }
}
