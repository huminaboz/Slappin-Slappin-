using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Debug_GrowthTester : MonoBehaviour
{
    [FormerlySerializedAs("upgrade")] [SerializeField] private SO_Upgrade upgradeSO;
    
    [SerializeField] private TextMeshProUGUI upgradeName;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI value;
    [SerializeField] private TextMeshProUGUI price;

    [SerializeField] [Range(0, 1)] private float updateValues;

    public string GetUpgradeText(int level)
    {
        float nextUpgrade = upgradeSO.newValueGrowthCurve.ComputeGrowth(upgradeSO.baseValue, level);
        
        switch (upgradeSO.numberType)
        {
            case NumberType.Normal:
                int roundedUpgrade = (int) Mathf.Ceil(nextUpgrade);
                if (roundedUpgrade <= level) roundedUpgrade = level + 1;
                roundedUpgrade--;
                return BozUtilities.FormatLargeNumber(roundedUpgrade);
            case NumberType.Percentage:
                return (nextUpgrade * 100).ToString("0.00") + "%";
            case NumberType.Multiplier:
                return nextUpgrade.ToString("0.00") + "x";
            default:
                return nextUpgrade.ToString();
        }
    }
    
    private void Update()
    {
        if (upgradeSO == null) return;
        upgradeName.text = upgradeSO.name;
        levelText.text = "Level";
        value.text = "Value";
        price.text = "Price";
        for (int level = 1; level < upgradeSO.maxLevel; level++)
        {
            levelText.text += "\n" + level;

            
            string newValue = GetUpgradeText(level);
            value.text += "\n" + newValue;
            float newPrice = upgradeSO.newPriceGrowthCurve.ComputeGrowth(upgradeSO.basePrice, level);
            newPrice = Mathf.Ceil(newPrice);
            price.text += "\n" + BozUtilities.FormatLargeNumber(newPrice);
        }
    }
}
