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
    
    public static IEnumerator DoAfterRealTimeDelay(float delay, Action action)
    {
        yield return new WaitForSecondsRealtime(delay);
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

    public static string GetUpgradeText(SO_Upgrade upgrade, int level, bool debugMode = false)
    {
        float nextUpgrade = upgrade.newValueGrowthCurve.ComputeGrowth(upgrade.baseValue, level);

        string output;

        switch (upgrade.numberType)
        {
            case NumberType.Normal:
                int roundedUpgrade = (int)Mathf.Ceil(nextUpgrade);
                //TODO:: Might need to make this actually take effect, too
                if (roundedUpgrade <= level) roundedUpgrade = level + 1;
                roundedUpgrade--;
                output = FormatLargeNumber(roundedUpgrade);
                break;
            case NumberType.Percentage:
                output = (nextUpgrade * 100).ToString("0.00") + "%";
                break;
            case NumberType.Multiplier:
                output = nextUpgrade.ToString("0.00") + "x";
                break;
            case NumberType.Seconds:
                output = nextUpgrade.ToString("0.00") + "s";
                break;
            default:
                output = nextUpgrade.ToString();
                break;
        }

        if (HasHitMinOrMax(upgrade, level))
        {
            if (debugMode)
            {
                output += $" MAX";
            }
            else output += $"\nMAX";
        }

        return output;
    }

    public static string FormatLargeNumber(float number, bool dontResizeLetter = false)
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
                return (number / 1_000).ToString("0.##") + "k";
            default:
                return number.ToString();
        }
    }
}