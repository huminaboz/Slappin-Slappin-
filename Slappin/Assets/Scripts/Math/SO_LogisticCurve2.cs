using UnityEngine;

[CreateAssetMenu(fileName = "SO_LogisticCurve_", menuName = "Slappin/SO_LogisticCurve")]
public class SO_LogisticCurve2 : SO_GrowthCurve
{
    public float logisticRate = 0.1f; // Rate of growth
    public float logisticCarryingCapacity = 100f; // Maximum capacity
    public float growthMultiplier = 1.05f; // Multiplier for each level
    public float baseGrowthRate = 1.0f; // Base growth rate to adjust growth at each level

    public override float ComputeGrowth(float baseValue, int level)
    {
        return 0f;
        // return baseValue + (1 + Mathf.Exp((-level)^-1;
    }
}
