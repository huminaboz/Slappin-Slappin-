using UnityEngine;

[CreateAssetMenu(fileName = "SO_ExponentialGrowth_", menuName = "Slappin/SO_ExponentialGrowth")]
public class SO_ExponentialCurve : SO_GrowthCurve
{
    [Range (0.0001f, 1f)] public float growthRate = 0.1f; // The exponential growth rate
    [Range (0.01f, 10f)] public float growthFactor = 1f;  // A multiplier to control the growth steepness
    public float offset = 0f;        // An offset to shift the growth curve
    
    public override float ComputeGrowth(float baseValue, int level)
    {
        // For level 1, return the base value directly
        if (level == 1)
        {
            return baseValue + offset; // Apply offset at level 1
        }
        
        float totalValue = baseValue; // Start with the base value at level 1

        // Loop to add the incremental value at each level
        for (int i = 2; i <= level; i++)
        {
            // Calculate the incremental value for the current level
            float increment = baseValue * Mathf.Exp(growthRate * growthFactor * (i - 1));

            // Add the increment to the total value
            totalValue += increment + offset;
        }

        return totalValue; // Return the cumulative value at the current level
    }
}
