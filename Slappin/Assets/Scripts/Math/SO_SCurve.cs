using UnityEngine;

[CreateAssetMenu(fileName = "SO_SCurve_", menuName = "Slappin/SO_SCurve")]
public class SO_SCurve : SO_GrowthCurve
{
    [SerializeField] private float initialGrowthAmplitude = 100f;     // Controls initial rapid growth amplitude
    [SerializeField] private float growthDecayRate = 10f;      // Controls how fast the rapid growth slows down
    [SerializeField] private float growthSteepness = 3f;       // Controls steepness of rapid growth
    [SerializeField] private float rateOfLongTermSlowGroth = 1f;       // Controls the rate of long-term slow growth
    
    
    public override float ComputeGrowth(float baseValue, int level)
    {
        return TotalGrowthAtLevel(level, baseValue);
    }
    
    
    // Function to calculate the incremental growth at each level
    private float IncrementalGrowth(int level)
    {
        // Step 1: Calculate the growth increment for this level
        float rapidGrowth = (initialGrowthAmplitude * Mathf.Pow(level, growthSteepness)) / (growthDecayRate + Mathf.Pow(level, growthSteepness));
        float slowGrowth = rateOfLongTermSlowGroth * Mathf.Log(level + 1);

        // Step 2: Return the total incremental value for this level
        return rapidGrowth + slowGrowth;
    }

    // Function to calculate the total cumulative value up to the current level
    private float TotalGrowthAtLevel(int currentLevel, float baseValue)
    {
        float totalValue = baseValue;

        // Iterate through each level up to the current level
        for (int level = 1; level <= currentLevel; level++)
        {
            totalValue += IncrementalGrowth(level);
        }

        return totalValue;
    }
}
