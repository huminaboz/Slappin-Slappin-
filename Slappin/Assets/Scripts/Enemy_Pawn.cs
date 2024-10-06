using UnityEngine;

public class Enemy_Pawn : MonoBehaviour, IDamageable
{
    public Health thisHealth { get; set; }

    public void AdjustHealth(int amount)
    {
    }

    public void HandleDeath()
    {
        //Throw up a puff of particle
        //Return to the pool
        
        Destroy(gameObject);
    }
}
