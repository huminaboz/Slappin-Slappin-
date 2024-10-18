using UnityEngine;

[CreateAssetMenu(fileName = "SO_LogisticCurve_", menuName = "Slappin/SO_LogisticCurve")]
public class SO_LogisticCurve : SO_GrowthCurve
{
    public float logisticRate = 0.1f;
    public float logisticCarryingCapacity = 100f;
    
    public override float ComputeGrowth(float baseValue, int level)
    {
        // Calculates the value based on logistic growth, which approaches a maximum carrying capacity.
        // The formula is: K / (1 + ((K - baseValue) / baseValue) * e^(-rate * (level - 1)))
        float exponent = -logisticRate * (level - 1);
        return logisticCarryingCapacity / (1 + ((logisticCarryingCapacity - baseValue) / baseValue) * Mathf.Exp(exponent));
    }
}
