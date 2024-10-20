using UnityEngine;
using UnityEngine.Serialization;

public enum NumberType
{
    Normal,
    Percentage, //Used for chances
    Multiplier, //1.1x and etc - percentage but over 100
    Seconds
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
    Luck,
    Enemy
}

public enum Stat
{
    None = 0,
    
    Enemy = 100,

    //Basic
    Basic = 1000,
    MoveBoostSpeed = 1001,
    AbsorbSpeed = 1002,

    //Defense
    Defense = 2000,
    
    IncreaseMaxHp = 2001,
    WaveHealthRestore = 2002,
    DamageReduction = 2003,

    //Slap
    Slap = 3000,
    SlapDamage = 3001,
    SlapAreaMultiplier = 3002,
    SlapDamagePerDistance = 3003,

    //Flick
    Flick = 4000,
    FlickMaxChargeDamage = 4001,
    FlickMaxChargeTime = 4002,

    //Squish
    Squish = 5000,
    SquishDamage = 5001,
    SquishDamagePerDistance = 5002,

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
    
    [Header("Min/Max - Can only use one")]
    [SerializeField] public bool useMinValue = false;
    [SerializeField] public bool useMaxValue = false;
    [SerializeField] public float minValue = 0;
    [SerializeField] public float maxValue = 999999999999;

    /// <summary>
    /// How much the value will increase with each level
    /// </summary>
    [Header("Growth Curves")]
    [SerializeField] public SO_GrowthCurve newValueGrowthCurve;


    /// <summary>
    /// How much the price will increase with each level
    /// </summary>
    [SerializeField] public SO_GrowthCurve newPriceGrowthCurve;

    [FormerlySerializedAs("maxLevels")] [SerializeField]
    public int maxLevel = 300;
}