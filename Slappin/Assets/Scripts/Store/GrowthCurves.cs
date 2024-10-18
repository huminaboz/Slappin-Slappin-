using UnityEngine;

public class GrowthCurves : Singleton<GrowthCurves>
{
    // Parameters for different growth curves, exposed to the Unity Inspector
    [Header("Linear Growth Parameters")]
    public float linearRate = 1f;

    [Header("Exponential Growth Parameters")]
    public float exponentialRate = 0.1f;

    [Header("Logistic Growth Parameters")]
    public float logisticRate = 0.1f;
    public float logisticCarryingCapacity = 100f;

    [Header("Gompertz Growth Parameters")]
    public float gompertzRate = 0.1f;
    public float gompertzCarryingCapacity = 100f;
    public float gompertzShift = 1f;

    [Header("Logarithmic Growth Parameters")]
    public float logarithmicScaleFactor = 10f;

    [Header("Polynomial Growth Parameters")]
    public float polynomialCoefficient = 1f;
    public int polynomialDegree = 2;

    [Header("Power Law Growth Parameters")]
    public float powerLawExponent = 2f;

    [Header("Stepwise Growth Parameters")]
    public float stepwiseStepSize = 10f;
    public int stepwiseStepInterval = 5;

    [Header("Damped Growth Parameters")]
    public float dampingFactor = 0.1f;
    public float oscillationFrequency = 1f;

    [Header("Hyperbolic Growth Parameters")]
    public float hyperbolicSingularityTime = 50f;

    [Header("Biexponential Growth Parameters")]
    public float biexponentialRate1 = 0.1f;
    public float biexponentialRate2 = 0.05f;
    public float biexponentialCoefficient = 0.5f;

    public enum GrowthCurveType
    {
        Linear,
        Exponential,
        Logistic,
        Gompertz,
        Logarithmic,
        Polynomial,
        PowerLaw,
        Stepwise,
        Damped,
        Hyperbolic,
        Biexponential
    }
    
    public float ComputeGrowth(float baseValue, int level, GrowthCurveType selectedGrowthCurve)
    {
        switch (selectedGrowthCurve)
        {
            case GrowthCurveType.Linear:
                return LinearGrowth(baseValue, level, linearRate);
            case GrowthCurveType.Exponential:
                return ExponentialGrowth(baseValue, level, exponentialRate);
            case GrowthCurveType.Logistic:
                return LogisticGrowth(baseValue, level, logisticRate, logisticCarryingCapacity);
            case GrowthCurveType.Gompertz:
                return GompertzGrowth(baseValue, level, gompertzRate, gompertzCarryingCapacity, gompertzShift);
            case GrowthCurveType.Logarithmic:
                return LogarithmicGrowth(baseValue, level, logarithmicScaleFactor);
            case GrowthCurveType.Polynomial:
                return PolynomialGrowth(baseValue, level, polynomialCoefficient, polynomialDegree);
            case GrowthCurveType.PowerLaw:
                return PowerLawGrowth(baseValue, level, powerLawExponent);
            case GrowthCurveType.Stepwise:
                return StepwiseGrowth(baseValue, level, stepwiseStepSize, stepwiseStepInterval);
            case GrowthCurveType.Damped:
                return DampedGrowth(baseValue, level, dampingFactor, oscillationFrequency);
            case GrowthCurveType.Hyperbolic:
                return HyperbolicGrowth(baseValue, level, hyperbolicSingularityTime);
            case GrowthCurveType.Biexponential:
                return BiexponentialGrowth(baseValue, level, biexponentialRate1, biexponentialRate2, biexponentialCoefficient);
            default:
                return baseValue; // Default case if no valid type is selected
        }
    }
    
    
    
    // Static functions for different growth curves
    public static float LinearGrowth(float baseValue, int level, float linearRate)
    {
        // Calculates the value based on linear growth.
        // The formula is: baseValue + rate * (level - 1)
        return baseValue + linearRate * (level - 1);
    }

    public static float ExponentialGrowth(float baseValue, int level, float exponentialRate)
    {
        // Calculates the value based on exponential growth.
        // The formula is: baseValue * e^(rate * (level - 1))
        return baseValue * Mathf.Exp(exponentialRate * (level - 1));
    }

    public static float LogisticGrowth(float baseValue, int level, float logisticRate, float logisticCarryingCapacity)
    {
        // Calculates the value based on logistic growth, which approaches a maximum carrying capacity.
        // The formula is: K / (1 + ((K - baseValue) / baseValue) * e^(-rate * (level - 1)))
        float exponent = -logisticRate * (level - 1);
        return logisticCarryingCapacity / (1 + ((logisticCarryingCapacity - baseValue) / baseValue) * Mathf.Exp(exponent));
    }

    public static float GompertzGrowth(float baseValue, int level, float gompertzRate, float gompertzCarryingCapacity, float gompertzShift)
    {
        // Calculates the value based on Gompertz growth, which is a type of logistic growth.
        // The formula is: K * e^(-e^(b - rate * (level - 1)))
        float exponent = gompertzShift - gompertzRate * (level - 1);
        return gompertzCarryingCapacity * Mathf.Exp(-Mathf.Exp(exponent));
    }

    public static float LogarithmicGrowth(float baseValue, int level, float logarithmicScaleFactor)
    {
        // Calculates the value based on logarithmic growth.
        // The formula is: baseValue + scaleFactor * log(level)
        return baseValue + logarithmicScaleFactor * Mathf.Log(level);
    }

    public static float PolynomialGrowth(float baseValue, int level, float polynomialCoefficient, int polynomialDegree)
    {
        // Calculates the value based on polynomial growth.
        // The formula is: baseValue + coefficient * (level ^ degree)
        return baseValue + polynomialCoefficient * Mathf.Pow(level, polynomialDegree);
    }

    public static float PowerLawGrowth(float baseValue, int level, float powerLawExponent)
    {
        // Calculates the value based on power-law growth.
        // The formula is: baseValue * (level ^ exponent)
        return baseValue * Mathf.Pow(level, powerLawExponent);
    }

    public static float StepwiseGrowth(float baseValue, int level, float stepwiseStepSize, int stepwiseStepInterval)
    {
        // Calculates the value based on stepwise growth, which increases at set intervals.
        // The formula is: baseValue + (level / stepInterval) * stepSize
        return baseValue + (level / stepwiseStepInterval) * stepwiseStepSize;
    }

    public static float DampedGrowth(float baseValue, int level, float dampingFactor, float oscillationFrequency)
    {
        // Calculates the value based on damped growth, which oscillates and reduces over time.
        // The formula is: baseValue * e^(-dampingFactor * (level - 1)) * cos(oscillationFrequency * (level - 1))
        return baseValue * Mathf.Exp(-dampingFactor * (level - 1)) * Mathf.Cos(oscillationFrequency * (level - 1));
    }

    public static float HyperbolicGrowth(float baseValue, int level, float hyperbolicSingularityTime)
    {
        // Calculates the value based on hyperbolic growth,
        // which approaches infinity as the level approaches singularityTime.
        // The formula is: baseValue / (singularityTime - (level - 1))
        return baseValue / (hyperbolicSingularityTime - (level - 1));
    }

    public static float BiexponentialGrowth(float baseValue, int level, float biexponentialRate1, float biexponentialRate2, float biexponentialCoefficient)
    {
        // Calculates the value based on biexponential growth, 
        //which combines two exponential growth rates.
        // The formula is: baseValue * e^(rate1 * (level - 1)) + coefficient * e^(rate2 * (level - 1))
        return baseValue * Mathf.Exp(biexponentialRate1 * (level - 1)) + biexponentialCoefficient * Mathf.Exp(biexponentialRate2 * (level - 1));
    }

    // Example of how to use these functions with public parameters in the Inspector
    public float ExampleLinearGrowth(float baseValue, int level)
    {
        return LinearGrowth(baseValue, level, linearRate);
    }

    public float ExampleExponentialGrowth(float baseValue, int level)
    {
        return ExponentialGrowth(baseValue, level, exponentialRate);
    }

    public float ExampleLogisticGrowth(float baseValue, int level)
    {
        return LogisticGrowth(baseValue, level, logisticRate, logisticCarryingCapacity);
    }

    public float ExampleGompertzGrowth(float baseValue, int level)
    {
        return GompertzGrowth(baseValue, level, gompertzRate, gompertzCarryingCapacity, gompertzShift);
    }

    public float ExampleLogarithmicGrowth(float baseValue, int level)
    {
        return LogarithmicGrowth(baseValue, level, logarithmicScaleFactor);
    }

    public float ExamplePolynomialGrowth(float baseValue, int level)
    {
        return PolynomialGrowth(baseValue, level, polynomialCoefficient, polynomialDegree);
    }

    public float ExamplePowerLawGrowth(float baseValue, int level)
    {
        return PowerLawGrowth(baseValue, level, powerLawExponent);
    }

    public float ExampleStepwiseGrowth(float baseValue, int level)
    {
        return StepwiseGrowth(baseValue, level, stepwiseStepSize, stepwiseStepInterval);
    }

    public float ExampleDampedGrowth(float baseValue, int level)
    {
        return DampedGrowth(baseValue, level, dampingFactor, oscillationFrequency);
    }

    public float ExampleHyperbolicGrowth(float baseValue, int level)
    {
        return HyperbolicGrowth(baseValue, level, hyperbolicSingularityTime);
    }

    public float ExampleBiexponentialGrowth(float baseValue, int level)
    {
        return BiexponentialGrowth(baseValue, level, biexponentialRate1, biexponentialRate2, biexponentialCoefficient);
    }
}
