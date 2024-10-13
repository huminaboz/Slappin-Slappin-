using System;
using System.Collections;
using UnityEngine;

public static class BozUtilities
{
    public static IEnumerator DoAfterDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
