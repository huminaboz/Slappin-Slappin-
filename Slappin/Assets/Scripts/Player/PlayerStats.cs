
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    //This should be a monohavior component on the player instead of a singleton
    [SerializeField] public float stunRecoveryMultiplier = 1f;
    
    public float currency1 = 0;
    public int currentWave = 1; //putting this here for now
    
    
    private void Start()
    {
        UpdateHUD();
    }

    //This really should be somewhere else, not the player stats
    public void AddCurency(int amount)
    {
        currency1 += amount;
        UpdateHUD();
        //todo:: Animate the number to show it went up
    }

    private void UpdateHUD()
    {
        GameplayUIManager.I.currency1.text = currency1.ToString();
    }
    
}
