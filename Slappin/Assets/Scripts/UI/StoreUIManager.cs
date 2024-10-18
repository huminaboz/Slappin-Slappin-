using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalCurrencyText;
    [SerializeField] private TextMeshProUGUI nextWaveButtonText;
    [SerializeField] private Button nextWaveButton;

    
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
}
