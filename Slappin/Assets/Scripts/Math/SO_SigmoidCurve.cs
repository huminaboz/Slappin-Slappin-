using UnityEngine;

[CreateAssetMenu(fileName = "SO_SigmoidGrowth_", menuName = "Slappin/SO_SigmoidGrowth")]
public class SO_SigmoidCurve : SO_GrowthCurve
{
    public float growthRate = 1f; // Rate of growth
    public float levelingFactor = 1f; // Controls how fast growth accelerates

    public override float ComputeGrowth(float baseValue, int level)
    {
        float growthValue = baseValue + Mathf.Exp(growthRate * level / levelingFactor);
        return growthValue;
    }
}
