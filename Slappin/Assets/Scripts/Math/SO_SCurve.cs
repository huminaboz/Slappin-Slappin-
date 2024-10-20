using UnityEngine;

[CreateAssetMenu(fileName = "SO_SCurve_", menuName = "Slappin/SO_SCurve")]
public class SO_SCurve : SO_GrowthCurve
{
    // [SerializeField] private float initialGrowthAmplitude = 100f; // Controls initial rapid growth amplitude
    [SerializeField] private  float growthRate = 0.5f;     // Affects how quickly the curve grows, smaller values make it gentler
    [SerializeField] private  float midpoint = 0f;         // The x-value where the curve's steep growth happens
    [SerializeField] private  float curveHeight = 10f;     // Maximum height the curve can add on top of the base value
    [SerializeField] private  float verticalScale = 1f;    // Scale to adjust the overall size of the curve

    
    public override float ComputeGrowth(float baseValue, int level)
    {
        return baseValue + (verticalScale * curveHeight / (1 + Mathf.Exp(-growthRate * (level - midpoint))));
    }
}