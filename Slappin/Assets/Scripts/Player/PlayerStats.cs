
using System;
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    //This should be a monohavior component on the player instead of a singleton
    [SerializeField] public float stunRecoveryMultiplier = 1f;
    
    public float currency1 = 0;
    

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

    public void UpdateHUD()
    {
        GameplayUIManager.I.currency1.text = BozUtilities.FormatLargeNumber(currency1);
        GameplayUIManager.I.currency1.gameObject.SetActive(false);
        GameplayUIManager.I.currency1.gameObject.SetActive(true);
    }
    
}
