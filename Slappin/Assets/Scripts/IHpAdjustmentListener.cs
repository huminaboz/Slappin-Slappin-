using UnityEngine;

public interface IHpAdjustmentListener
{
    //This idea here is that on any type of thing that can die I can add an 
    //IDamageable and then fill in what logic happens  when they die
    public void TookDamage(int damageAmount, GameObject attacker);
    public void Healed(int healAmount, GameObject healer);
    public float HandleDeath(int lastAttack, GameObject killer);
}
