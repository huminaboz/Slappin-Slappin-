using System;
using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StoreUIManager : MonoBehaviour
{
    [SerializeField] private GameObject rowsParent;
    [SerializeField] private GameObject categoryRowPrefab;
    [SerializeField] private UpgradeData upgradeDataPrefab;

    [SerializeField] private TextMeshProUGUI totalCurrencyText;
    [SerializeField] private TextMeshProUGUI nextWaveButtonText;
    [SerializeField] private Button nextWaveButton;

    [SerializeField] private GameObject categoryRowTitlePrefab;

    public delegate void DebugUpdateStoreUI();

    public static event DebugUpdateStoreUI OnDebugUpdateStoreUI;

    //TODO:: Populate all the categories from the resources folder scrobs

    private void OnEnable()
    {
        UpgradeData.OnPurchaseMade += UpdateLabels;
    }

    private void OnDisable()
    {
        UpgradeData.OnPurchaseMade -= UpdateLabels;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        UpdateLabels();
        BuildCategories();
    }

    private void UpdateLabels()
    {
        nextWaveButtonText.text = "Start Wave " + PlayerStats.I.currentWave;
        totalCurrencyText.text = BozUtilities.FormatLargeNumber(PlayerStats.I.currency1);
        // totalCurrencyText.text = PlayerStats.I.currency1.ToString("F0");
    }

    Dictionary<UpgradeType, GameObject> categories = new Dictionary<UpgradeType, GameObject>();
    List<UpgradeType> upgradeTypes = new List<UpgradeType>();

    private Transform GetCategoryParent(UpgradeType upgradeType)
    {
        if (categories.TryGetValue(upgradeType, out GameObject categoryRow))
        {
            return categoryRow.transform;
        }

        Debug.LogError("No category found for upgrade type: " + upgradeType);
        return null;
    }

    private void BuildCategories()
    {
        string resourcePath = "UpgradeScrobs";
        SO_Upgrade[] upgrades = Resources.LoadAll<SO_Upgrade>(resourcePath);

        foreach (SO_Upgrade upgradeSO in upgrades)
        {
            if (upgradeTypes.Contains(upgradeSO.upgradeType)) continue;
            upgradeTypes.Add(upgradeSO.upgradeType);
            GameObject categoryRow = Instantiate(categoryRowPrefab, rowsParent.transform);
            categories.Add(upgradeSO.upgradeType, categoryRow);

            //Add the category title
            GameObject categoryRowTitle = Instantiate(categoryRowTitlePrefab, categoryRow.transform);
            TextMeshProUGUI categoryRowTitleText = categoryRowTitle.GetComponentInChildren<TextMeshProUGUI>();
            categoryRowTitleText.text = upgradeSO.upgradeType.ToString();
        }

        // Iterate through each SO_Upgrade and instantiate an UpgradeData prefab for each
        foreach (SO_Upgrade upgradeSO in upgrades)
        {
            UpgradeData upgradeData = Instantiate(upgradeDataPrefab, 
                GetCategoryParent(upgradeSO.upgradeType));

            // Assign the SO_Upgrade to the upgradeData's upgradeSO field
            if (upgradeData != null)
            {
                upgradeData.upgradeSO = upgradeSO;
            }
            else
            {
                Debug.LogError("UpgradeData component not found on prefab!");
            }
        }
    }

    [Command]
    private void DebugAddMoney(float amount)
    {
        PlayerStats.I.currency1 += amount;
        UpdateLabels();
        OnDebugUpdateStoreUI?.Invoke();
    }
}