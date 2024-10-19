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

public enum Stat
{
    None = 0,
    
    //Basic
    Basic = 1000,
    DamageReduction = 1001,
    
    //Defense
    Defense = 2000,
    
    
    //Slap
    Slap = 3000,
    SlapDamage = 3001,
    SlapArea = 3002,
    
    //Flick
    Flick = 4000,
    
    
    //Squish
    Squish = 5000,
    
    
    //Wild
    Wild = 6000,
    
    
    //Fart
    Fart = 7000,
    
    
    //Luck
    Luck = 8000,
    
    
}

[CreateAssetMenu(fileName = "SO_Upgrade_", menuName = "Slappin/SO_Upgrade")]
public class SO_Upgrade : ScriptableObject
{
    [SerializeField] public Stat stat = 0;
    [SerializeField] public string title;
    [SerializeField] public float baseValue = 1;
    [SerializeField] public int basePrice = 5;
    [SerializeField] public UpgradeType upgradeType;
    [SerializeField] public NumberType numberType;

    /// <summary>
    /// How much the value will increase with each level
    /// </summary>
    [SerializeField] public SO_GrowthCurve newValueGrowthCurve;
    
    
    /// <summary>
    /// How much the price will increase with each level
    /// </summary>
    [SerializeField] public SO_GrowthCurve newPriceGrowthCurve;

    [FormerlySerializedAs("maxLevels")] [SerializeField] public int maxLevel = 300;
    
    
}
