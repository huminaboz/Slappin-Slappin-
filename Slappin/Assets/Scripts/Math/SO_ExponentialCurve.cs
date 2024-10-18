using UnityEngine;

[CreateAssetMenu(fileName = "SO_ExponentialGrowth_", menuName = "Slappin/SO_ExponentialGrowth")]
public class SO_ExponentialCurve : SO_GrowthCurve
{
    public float exponentialRate = 0.1f;
    
    public override float ComputeGrowth(float baseValue, int level)
    {
        return baseValue * Mathf.Exp(exponentialRate * (level - 1));
    }
}
