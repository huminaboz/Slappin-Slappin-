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

            
            string newValue = BozUtilities.GetUpgradeText(upgradeSO, level);
            value.text += "\n" + newValue;
            float newPrice = upgradeSO.newPriceGrowthCurve.ComputeGrowth(upgradeSO.basePrice, level);
            newPrice = Mathf.Ceil(newPrice);
            price.text += "\n" + BozUtilities.FormatLargeNumber(newPrice);
        }
    }
}
