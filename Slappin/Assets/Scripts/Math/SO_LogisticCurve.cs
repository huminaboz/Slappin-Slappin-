using UnityEngine;

[CreateAssetMenu(fileName = "SO_LogisticCurve_", menuName = "Slappin/SO_LogisticCurve")]
public class SO_LogisticCurve : SO_GrowthCurve
{
    public float logisticRate = 0.1f; // Rate of growth
    public float logisticCarryingCapacity = 100f; // Maximum capacity
    public float growthMultiplier = 1.05f; // Multiplier for each level
    public float baseGrowthRate = 1.0f; // Base growth rate to adjust growth at each level

    public override float ComputeGrowth(float baseValue, int level)
    {
        // Initialize cumulative base value
        float cumulativeBaseValue = baseValue;

        // Calculate growth for each level
        for (int i = 1; i <= level; i++)
        {
            // Calculate the growth at the current level
            float exponent = -logisticRate * (i - 1);
            float currentGrowth = (logisticCarryingCapacity / 
                                   (1 + ((logisticCarryingCapacity - cumulativeBaseValue) / cumulativeBaseValue) * Mathf.Exp(exponent))) * growthMultiplier;
        
            // Apply the base growth rate
            cumulativeBaseValue += currentGrowth * baseGrowthRate;
        }

        return cumulativeBaseValue;
    }
}
