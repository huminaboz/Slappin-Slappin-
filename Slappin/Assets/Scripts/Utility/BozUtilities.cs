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
                return (number / 1_000).ToString("0.##") + "k";
            default:
                return number.ToString();
        }
    }
}
