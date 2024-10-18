using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


//For now I think I Can handle all of these with a curve
// public enum GrowthAlgorithmTypes
// {
//     Linear,
//     Exponential,
//     Logarithmic,
// }

public enum NumberType
{
    Normal,
    Percentage, //Used for chances
    Multiplier //1.1x and etc - percentage but over 100
}

public enum UpgradeType
{
    Basic,
    Defense,
    Slap,
    Flick,
    Squish,
    Fart,
    Wild,
    Luck
}

[CreateAssetMenu(fileName = "SO_Upgrade_", menuName = "Slappin/SO_Upgrade")]
public class SO_Upgrade : ScriptableObject
{
    [SerializeField] public string title;
    [SerializeField] public float baseValue = 1;
    [SerializeField] public int basePrice = 5;
    [SerializeField] public UpgradeType upgradeType;
    [SerializeField] public NumberType numberType;

    /// <summary>
    /// How much the value will increase with each level
    /// </summary>
    [SerializeField] public GrowthCurves.GrowthCurveType valueGrowthCurve;
    // [SerializeField] public AnimationCurve valueGrowthCurve;

    /// <summary>
    /// How much the price will increase with each level
    /// </summary>
    [SerializeField] public GrowthCurves.GrowthCurveType priceGrowthCurve;
    // [SerializeField] public AnimationCurve priceGrowthCurve;

    [FormerlySerializedAs("maxLevels")] [SerializeField] public int maxLevel = 300;
}