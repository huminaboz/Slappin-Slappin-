using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public static class BozUtilities
{
    public static IEnumerator DoAfterDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    public static float GetDiceRoll()
    {
        return Random.Range(0f, 1f);
    }
    
    public static bool HasHitMinOrMax(SO_Upgrade upgrade, int _level)
    {
        if (upgrade.useMinValue)
        {
            if (upgrade.minValue >= upgrade.newValueGrowthCurve.ComputeGrowth(upgrade.baseValue, _level))
            {
                //TODO:: Do something special about max level reached
                return true;
            }
        }
        else if (upgrade.useMaxValue)
        {
            if (upgrade.maxValue <= upgrade.newValueGrowthCurve.ComputeGrowth(upgrade.baseValue, _level))
            {
                //TODO:: Do something special about max level reached
                return true;
            }
        }

        return false;
    }
    
    public static string GetUpgradeText(SO_Upgrade upgrade, int level)
    {
        float nextUpgrade = upgrade.newValueGrowthCurve.ComputeGrowth(upgrade.baseValue, level);

        if (HasHitMinOrMax(upgrade, level)) return $"{nextUpgrade} MIN/MAX";
        
        switch (upgrade.numberType)
        {
            case NumberType.Normal:
                int roundedUpgrade = (int) Mathf.Ceil(nextUpgrade);
                //TODO:: Might need to make this actually take effect, too
                if (roundedUpgrade <= level) roundedUpgrade = level + 1;
                roundedUpgrade--;
                return FormatLargeNumber(roundedUpgrade);
            case NumberType.Percentage:
                return (nextUpgrade * 100).ToString("0.00") + "%";
            case NumberType.Multiplier:
                return nextUpgrade.ToString("0.00") + "x";
            case NumberType.Seconds:
                return nextUpgrade.ToString("0.00") + "s";
            default:
                return nextUpgrade.ToString();
        }
    }
    
    public static string FormatLargeNumber(float number)
    {
        switch (number)
        {
            case >= 1_000_000_000_000_000_000:
                return (number / 1_000_000_000_000_000_000).ToString("0.##") + "E";
            case >= 1_000_000_000_000_000:
                return (number / 1_000_000_000_000_000).ToString("0.##") + "Q";
            case >= 1_000_000_000_000:
                return (number / 1_000_000_000_000).ToString("0.##") + "T";
            case >= 1_000_000_000:
                return (number / 1_000_000_000).ToString("0.##") + "B";
            case >= 1_000_000:
                return (number / 1_000_000).ToString("0.##") + "M";
            case >= 1_000:
                return (number / 1_000).ToString("0.##") + "<size=70%>k</font>";
            default:
                return number.ToString();
        }
    }
}
