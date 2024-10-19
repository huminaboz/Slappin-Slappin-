using UnityEngine;

[CreateAssetMenu(fileName = "SO_LinearGrowth_", menuName = "Slappin/SO_LinearGrowth")]
public class SO_LinearGrowth : SO_GrowthCurve
{
    [Header("Linear Growth")] 
    public float linearRate = 1f;
    
    public override float ComputeGrowth(float baseValue, int level)
    {
        return baseValue + linearRate * (level - 1);
    }
}
