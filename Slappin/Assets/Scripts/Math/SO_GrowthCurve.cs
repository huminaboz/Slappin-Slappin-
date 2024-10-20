using UnityEngine;
using UnityEngine.Serialization;

public class SO_GrowthCurve : ScriptableObject
{
    [TextArea(3,100)][SerializeField] private string notes;
    
    [Header("Preview Values")] [SerializeField]
    private bool showPreview;

    [SerializeField] private SO_Upgrade previewUpgrade;
    [SerializeField] private int maxDisplayedRows = 34;
    [Range(0, 1000)] [SerializeField] private int startOffset = 1;
    [SerializeField] [TextArea(1, 200)] private string debugValues;


    public virtual float ComputeGrowth(float baseValue, int level)
    {
        return 0;
    }

    private void OnValidate()
    {
        if (!showPreview || previewUpgrade is null) return;
        debugValues = "";

        debugValues += "Level";
        debugValues += "\t\t";
        debugValues += "Amount";
        debugValues += "\t\t\t";
        debugValues += "Price";
        debugValues += "\n";

        for (int level = startOffset; level < maxDisplayedRows + startOffset; level++)
        {
            debugValues += level.ToString();
            debugValues += "\t\t";
            string newValue = BozUtilities.GetUpgradeText(previewUpgrade, level);
            debugValues += newValue;
            debugValues += "\t\t\t";
            float newPrice = previewUpgrade.newPriceGrowthCurve.ComputeGrowth(previewUpgrade.basePrice, level);
            newPrice = Mathf.Ceil(newPrice);
            debugValues += BozUtilities.FormatLargeNumber(newPrice);
            debugValues += "\n";
        }
    }
}