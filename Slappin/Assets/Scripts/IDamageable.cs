using UnityEngine;

public interface IDamageable
{
    //This idea here is that on any type of thing with health I can add an IDamageable and then fill in what logic happens when they take damage and when they die
    public Health thisHealth { get; set; }

    public void AdjustHealth(int amount);

    public void HandleDeath();
}
