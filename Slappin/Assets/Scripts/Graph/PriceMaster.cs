using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriceMaster : MonoBehaviour
{
    [SerializeField] private SO_Upgrade priceUpgrader;

    public float GetPriceAtLevel(int level)
    {
        return priceUpgrader.newValueGrowthCurve.ComputeGrowth(priceUpgrader.baseValue, level)
               * priceUpgrader.baseValueForMultiplier;
    }
}