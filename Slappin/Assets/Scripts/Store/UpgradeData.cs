using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Serialization;

public class UpgradeData : MonoBehaviour
{
    //This is where the math and action happens
    //Should add this component to the upgrade card on creation 
    [SerializeField] public SO_Upgrade upgradeSO;

    private UpgradeCard_Appearance _upgradeCardAppearance;

    public delegate void OnBoughtSomething();

    public static event OnBoughtSomething OnPurchaseMade;

    public int level = 1;
    // public 

    private void Awake()
    {
        _upgradeCardAppearance = GetComponentInChildren<UpgradeCard_Appearance>();
    }

    private void Start()
    {
        _upgradeCardAppearance.Initialize();
    }

    public void OnPurchased()
    {
        //Check to see if you can afford this, and if not, disallow purchase   
        if (IsAllowedToBePurchased() == false)
        {
            Debug.LogWarning("You can't afford that!");
            return;
        }


        PlayerStats.I.currency1 -= GetPrice();

        //NOTE: Remember that by incrementing this, it will increase everything, so updates after, purchases before
        level++;
        //Upgrade the universal source of truth for getting stat numbers
        StatLiason.I.Stats[upgradeSO.stat]
            = upgradeSO.newValueGrowthCurve.ComputeGrowth(upgradeSO.baseValue, level);

        //Send out an event to update all the cards appearances for affordability or not
        OnPurchaseMade?.Invoke();
    }

    public bool IsAllowedToBePurchased()
    {
        return GetPrice() <= PlayerStats.I.currency1
               && level < upgradeSO.maxLevel;
    }

    private int GetPrice()
    {
        return (int)Mathf.Floor(upgradeSO.newPriceGrowthCurve.ComputeGrowth(upgradeSO.basePrice, level));
    }

    public string GetPriceText()
    {
        return BozUtilities.FormatLargeNumber(GetPrice());
    }
}