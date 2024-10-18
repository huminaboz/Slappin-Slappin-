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
        return (int) GrowthCurves.I.ComputeGrowth(upgradeSO.basePrice, level, upgradeSO.priceGrowthCurve);
    }

    public string GetPriceText()
    {
        return FormatLargeNumber(GetPrice());
    }


    public string GetUpgradeText()
    {
        switch (upgradeSO.numberType)
        {
            case NumberType.Normal:
                return FormatLargeNumber(upgradeSO.baseValue);
            case NumberType.Percentage:
                return (upgradeSO.baseValue * 100).ToString("0.00") + "%";
            case NumberType.Multiplier:
                return upgradeSO.baseValue.ToString("0.00") + "x";
            default:
                return upgradeSO.ToString();
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

    static string FormatLargeNumber(float number)
    {
        if (number >= 1_000_000_000)
        {
            return (number / 1_000_000_000).ToString("0.##") + "B";
        }
        else if (number >= 1_000_000)
        {
            return (number / 1_000_000).ToString("0.##") + "M";
        }
        else if (number >= 1_000)
        {
            return (number / 1_000).ToString("0.##") + "k";
        }
        else
        {
            return number.ToString();
        }
    }
}