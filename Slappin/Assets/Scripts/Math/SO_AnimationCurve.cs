using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SO_AnimationCurve_", menuName = "Slappin/SO_AnimationCurve")]
public class SO_AnimationCurve : SO_GrowthCurve
{
    [SerializeField] public AnimationCurve AnimationCurve;
    [SerializeField] public int levelAtWhichGraphReaches1;
    [SerializeField] public float multiplier = 1f;

    public override float ComputeGrowth(float baseValue, int level)
    {
        float totalValue = baseValue;

        for (int i = 0; i < level; i++)
        {
            float ratio = i / (float)levelAtWhichGraphReaches1;
            totalValue += multiplier * AnimationCurve.Evaluate(ratio);
        }

        return totalValue;
    }
}