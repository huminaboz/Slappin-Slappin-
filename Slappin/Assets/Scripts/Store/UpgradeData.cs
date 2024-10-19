using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Mono.CSharp;
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


    private void Awake()
    {
        _upgradeCardAppearance = GetComponentInChildren<UpgradeCard_Appearance>();
    }

    private void Start()
    {
        _upgradeCardAppearance.Initialize();
    }



    public void OnAttemptedToPurchase()
    {
        int amount = 1;
        
        if (Input.GetButton("Fire3") || Input.GetKey(KeyCode.LeftShift))
        {
            amount = 10;
        }
        
        //Check to see if you can afford this, and if not, disallow purchase   
        if (IsAllowedToBePurchased(amount) == false)
        {
            Debug.LogWarning("You can't afford that!");
            return;
        }

        PlayerStats.I.currency1 -= GetPrice(amount);

        //NOTE: Remember that by incrementing this, it will increase everything, so updates after, purchases before
        level++;
        //Upgrade the universal source of truth for getting stat numbers
        StatLiason.I.Stats[upgradeSO.stat]
            = upgradeSO.newValueGrowthCurve.ComputeGrowth(upgradeSO.baseValue, level);

        //Send out an event to update all the cards appearances for affordability or not
        OnPurchaseMade?.Invoke();
    }

    public bool IsAllowedToBePurchased(int amount)
    {
        if (BozUtilities.HasHitMinOrMax(upgradeSO, level)) return false;

        return GetPrice(amount) <= PlayerStats.I.currency1
               && level + (amount - 1) < upgradeSO.maxLevel;
    }


    private int GetPrice(int amount)
    {
        return (int)Mathf.Floor(upgradeSO.newPriceGrowthCurve
            .ComputeGrowth(upgradeSO.basePrice, level + (amount - 1)));
    }

    public string GetPriceText(int amount)
    {
        return BozUtilities.FormatLargeNumber(GetPrice(amount));
    }
}