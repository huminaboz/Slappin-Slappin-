using System;
using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalCurrencyText;
    [SerializeField] private TextMeshProUGUI nextWaveButtonText;
    [SerializeField] private Button nextWaveButton;

    
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
    }

    private void UpdateLabels()
    {
        nextWaveButtonText.text = "Start Wave " + PlayerStats.I.currentWave;
        totalCurrencyText.text = PlayerStats.I.currency1.ToString();
    }

    [Command]
    private void DebugAddMoney(int amount)
    {
        PlayerStats.I.currency1 += amount;
        UpdateLabels();
        OnDebugUpdateStoreUI?.Invoke();
    }
}
