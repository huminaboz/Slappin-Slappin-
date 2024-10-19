using System;
using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StoreUIManager : Singleton<StoreUIManager>
{
    [SerializeField] private GameObject rowsParent;
    [SerializeField] private GameObject categoryRowPrefab;
    [SerializeField] private UpgradeData upgradeDataPrefab;

    [SerializeField] private TextMeshProUGUI totalCurrencyText;
    [SerializeField] private TextMeshProUGUI nextWaveButtonText;
    [SerializeField] private Button nextWaveButton;

    [SerializeField] private GameObject categoryRowTitlePrefab;

    [SerializeField] private Color colorBasic;
    [SerializeField] private Color colorDefense;
    [SerializeField] private Color colorSlap;
    [SerializeField] private Color colorFlick;
    [SerializeField] private Color colorSquish;
    [SerializeField] private Color colorFart;
    [SerializeField] private Color colorWild;
    [SerializeField] private Color colorLuck;
    
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

    public void ExitStore()
    {
        UIStateSwapper.I.SetState(UIStateSwapper.UIState.playing);
    }

    private void UpdateLabels()
    {
        nextWaveButtonText.text = "Start Wave " + PlayerStats.I.currentWave;
        totalCurrencyText.text = BozUtilities.FormatLargeNumber(PlayerStats.I.currency1);
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
            categoryRowTitle.GetComponent<Image>().color = GetCategoryColor(upgradeSO.upgradeType);
            TextMeshProUGUI categoryRowTitleText = categoryRowTitle.GetComponentInChildren<TextMeshProUGUI>();
            categoryRowTitleText.text = upgradeSO.upgradeType.ToString();
        }

        // Iterate through each SO_Upgrade and instantiate an UpgradeData prefab for each
        foreach (SO_Upgrade upgradeSO in upgrades)
        {
            StatLiason.I.Stats.Add(upgradeSO.stat, upgradeSO.baseValue);
            if (upgradeSO.stat == 0)
            {
                Debug.LogError("You forgot to set a stat enum on: " + upgradeSO.title);
            }
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

    public Color GetCategoryColor(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.Basic:
                return colorBasic;
            case UpgradeType.Defense:
                return colorDefense;
            case UpgradeType.Slap:
                return colorSlap;
            case UpgradeType.Flick:
                return colorFlick;
            case UpgradeType.Squish:
                return colorSquish;
            case UpgradeType.Fart:
                return colorFart;
            case UpgradeType.Wild:
                return colorWild;
            case UpgradeType.Luck:
                return colorLuck;
            default:
                throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null);
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