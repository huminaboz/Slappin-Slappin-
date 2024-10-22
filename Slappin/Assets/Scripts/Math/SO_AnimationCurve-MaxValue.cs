using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SO_AnimationCurve-MaxValue", menuName = "Slappin/SO_AnimationCurve-MaxValue")]
public class SO_AnimationCurveMaxValue : SO_GrowthCurve
{
    [SerializeField] public AnimationCurve AnimationCurve;
    [SerializeField] public int levelAtWhichGraphReaches1;
    [FormerlySerializedAs("multiplier")] [SerializeField] public float maximumValue = 1f;
    [SerializeField] private float multiplier = 1f;
    
    
    public override float ComputeGrowth(float baseValue, int level)
    {
        float totalValue = baseValue;

            float ratio = level / (float)levelAtWhichGraphReaches1;
            totalValue += (maximumValue-baseValue) * AnimationCurve.Evaluate(ratio);
            totalValue *= multiplier;

        return totalValue;
    }
}