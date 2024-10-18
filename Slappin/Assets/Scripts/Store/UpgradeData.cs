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


    public delegate void OnBoughtSomething();

    public static event OnBoughtSomething OnPurchaseMade;

    public int level = 1;
    // public 

    public void OnPurchased()
    {
        //Check to see if you can afford this, and if not, disallow purchase   
        if (IsAllowedToBePurchased() == false)
        {
            Debug.LogWarning("You can't afford that!");
            return;
        }

        //TODO:: Take care of adding all the stuff
        
        
        PlayerStats.I.currency1 -= GetPrice();
        
        //Remember that by incrementing this, it will increase everything, so updates after, purchases before
        level++;
        
        //Send out an event to update all the cards appearances for affordability or not
        OnPurchaseMade?.Invoke();
    }

    public bool IsAllowedToBePurchased()
    {
        return GetPrice() <= PlayerStats.I.currency1
               &&  level < upgradeSO.maxLevel;
    }

    private int GetPrice()
    {
        return (int) Mathf.Floor(upgradeSO.newPriceGrowthCurve.ComputeGrowth(upgradeSO.basePrice, level));
    }

    public string GetPriceText()
    {
        return BozUtilities.FormatLargeNumber(GetPrice());
    }


    public string GetUpgradeText()
    {
        float nextUpgrade = upgradeSO.newValueGrowthCurve.ComputeGrowth(upgradeSO.baseValue, level);
        
        switch (upgradeSO.numberType)
        {
            case NumberType.Normal:
                int roundedUpgrade = (int) Mathf.Ceil(nextUpgrade);
                if (roundedUpgrade <= level) roundedUpgrade = level + 1;
                roundedUpgrade--;
                return BozUtilities.FormatLargeNumber(roundedUpgrade);
            case NumberType.Percentage:
                return (nextUpgrade * 100).ToString("0.00") + "%";
            case NumberType.Multiplier:
                return nextUpgrade.ToString("0.00") + "x";
            default:
                return nextUpgrade.ToString();
        }
    }

    // public Color GetPriceBgColor()
    // {
    //     switch (upgradeSO.upgradeType)
    //     {
    //         case UpgradeType.Basic:
    //             break;
    //         case UpgradeType.Defense:
    //             break;
    //         case UpgradeType.Slap:
    //             break;
    //         case UpgradeType.Flick:
    //             break;
    //         case UpgradeType.Squish:
    //             break;
    //         case UpgradeType.Fart:
    //             break;
    //         case UpgradeType.Wild:
    //             break;
    //         case UpgradeType.Luck:
    //             break;
    //         default:
    //             throw new ArgumentOutOfRangeException();
    //     }
    // }


}