using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(UpgradeData))]
public class UpgradeCard_Appearance : MonoBehaviour
{
    private UpgradeData upgradeData;
    private Button thisButton;

    //Text stuff
    [SerializeField] public TextMeshProUGUI priceText;
    [SerializeField] public TextMeshProUGUI upgradeText;
    [SerializeField] public TextMeshProUGUI titleText;


    //Background
    [SerializeField] public Image priceBgColor;
    [SerializeField] private GameObject border;
    [SerializeField] private Image shadowImage;

    private Color shadowDefault;
    [SerializeField] private Color shadowSelected;
    [SerializeField] private Color shadowPressed;
    [SerializeField] private Color priceBgDisabledColor;
    [SerializeField] private Color priceTextDisabledColor;

    [SerializeField] private RectTransform cardBodyRect;

    private Vector2 _bodyDefaultPosition;
    private Color _defaultPriceBgColor;
    private Color _defaultPriceTextColor;


    //TODO:: This will get called when opening the menu in the future
    public void Initialize()
    {
        upgradeData = GetComponent<UpgradeData>();
        shadowDefault = shadowImage.color;
        _bodyDefaultPosition = cardBodyRect.anchoredPosition;
        _defaultPriceBgColor = StoreUIManager.I.GetCategoryColor(upgradeData.upgradeSO.upgradeType);
        _defaultPriceTextColor = priceText.color;
        thisButton = GetComponent<Button>();
        gameObject.name = upgradeData.upgradeSO.title;
        titleText.text = upgradeData.upgradeSO.title;
        UpdateCardAppearance();
    }

    private void OnEnable()
    {
        UpgradeData.OnPurchaseMade += UpdateCardAppearance;
        StoreUIManager.ChangedPreviewAmount += UpdateCardAppearance;
        StoreUIManager.OnDebugUpdateStoreUI += UpdateCardAppearance;
    }

    private void OnDisable()
    {
        UpgradeData.OnPurchaseMade -= UpdateCardAppearance;
        StoreUIManager.ChangedPreviewAmount -= UpdateCardAppearance;
        StoreUIManager.OnDebugUpdateStoreUI -= UpdateCardAppearance;
    }

    private void UpdateCardAppearance()
    {
        priceText.text = upgradeData.GetPriceText(StoreUIManager.I.previewAmount);
        upgradeText.text = BozUtilities.GetUpgradeText(upgradeData.upgradeSO,
            upgradeData.level + 1 + StoreUIManager.I.previewAmount);

        //TODO:: Set up a singleton or something that has all the category colors
        // priceBgColor.color = upgradeData.upgradeSO.GetCategoryColor;
        if (upgradeData.IsAllowedToBePurchased(StoreUIManager.I.previewAmount))
        {
            SetDefaultAppearance();
        }
        else
        {
            OnCantBePurchased();
        }
    }

    private void OnCantBePurchased()
    {
        priceBgColor.color = priceBgDisabledColor;
        priceText.color = priceTextDisabledColor;

        //Don't let the button get pressed
        thisButton.interactable = false;
    }

    public void SetDefaultAppearance()
    {
        border.gameObject.SetActive(false);
        shadowImage.color = shadowDefault;
        cardBodyRect.anchoredPosition = _bodyDefaultPosition;

        //Don't let the button events set this if there's no moneys
        if (upgradeData.IsAllowedToBePurchased(StoreUIManager.I.previewAmount))
        {
            priceBgColor.color = _defaultPriceBgColor;
            priceText.color = _defaultPriceTextColor;

            //Allow the button to be pressed
            thisButton.interactable = true;
        }
        //Do something special if the amount is maxed out
        else
        {
            if (BozUtilities.HasHitMinOrMax(upgradeData.upgradeSO, upgradeData.level))
            {
                priceText.text = "MAX";
            }
        }
    }

    public void OnSelected()
    {
        if (upgradeData.IsAllowedToBePurchased(StoreUIManager.I.previewAmount))
        {
            SFXPlayer.I.Play(AudioEventsStorage.I.HoverUpgrade);
        }

        border.gameObject.SetActive(true);
        shadowImage.color = shadowSelected;
        cardBodyRect.anchoredPosition = _bodyDefaultPosition;
    }

    public void OnPressed()
    {
        if (upgradeData.IsAllowedToBePurchased(StoreUIManager.I.previewAmount) == false) return;
        cardBodyRect.anchoredPosition = new Vector2(10f, -10f);
        border.gameObject.SetActive(true);
        shadowImage.color = shadowPressed;
    }
}