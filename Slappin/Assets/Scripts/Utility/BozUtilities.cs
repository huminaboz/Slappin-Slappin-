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
}
