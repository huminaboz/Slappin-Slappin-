using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SO_AnimationCurve_", menuName = "Slappin/SO_AnimationCurve")]
public class SO_AnimationCurve : SO_GrowthCurve
{
    [SerializeField] public AnimationCurve AnimationCurve;
    [SerializeField] public int levelAtWhichGraphReaches1;


    public override float ComputeGrowth(float baseValue, int level)
    {
        float asdf = baseValue + baseValue * AnimationCurve.Evaluate((float)level / levelAtWhichGraphReaches1);
        return asdf;
    }
}