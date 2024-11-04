using System;
using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
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

    [SerializeField] private TextMeshProUGUI buyModeText;
    public int previewAmount = 1;
    public static Action ChangedPreviewAmount;

    [FormerlySerializedAs("UpgradeCards")] public List<GameObject> UpgradeCardButtons = new List<GameObject>();
    [SerializeField] private Button startWaveButton;
    [SerializeField] public QuantumConsole _qc;

    private InputSystem_Actions _inputSystem;
    
    public delegate void DebugUpdateStoreUI();

    public static event DebugUpdateStoreUI OnDebugUpdateStoreUI;

    //TODO:: Populate all the categories from the resources folder scrobs

    private void Awake()
    {
        Initialize();
        _inputSystem = new InputSystem_Actions();
        _inputSystem.Player.Enable();
    }
    
    private void OnEnable()
    {
        UpgradeData.OnPurchaseMade += UpdateLabels;
        _qc.OnDeactivate += OnEnteredStore;
    }

    private void OnDisable()
    {
        UpgradeData.OnPurchaseMade -= UpdateLabels;
        _qc.OnDeactivate -= OnEnteredStore;
        _inputSystem.Player.Disable();
    }


    private void Start()
    {
        MusicPlayer.I.Play(AudioEventsStorage.I.playing);
    }

    private void Initialize()
    {
        UpdateLabels();
        BuildCategories();
    }

    private void Update()
    {
        if (_inputSystem.Player.Fart.WasPressedThisFrame())
        {
            //TODO:: Update the price previews
            previewAmount = 10;
            UpdateBuyModeText(previewAmount);
            ChangedPreviewAmount?.Invoke();
        }

        if (_inputSystem.Player.Fart.WasReleasedThisFrame())
        {
            //TODO:: Update the price previews
            previewAmount = 1;
            UpdateBuyModeText(previewAmount);
            ChangedPreviewAmount?.Invoke();
        }
    }

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;

    public void ScrollTo(RectTransform target)
    {
        // Reset the scroll position
        scrollRect.verticalNormalizedPosition = 1f; // Top position

        // Force update the canvas to apply the reset position
        Canvas.ForceUpdateCanvases();

        // Calculate the new position
        Vector2 viewportPosition = scrollRect.viewport.InverseTransformPoint(scrollRect.viewport.position);
        Vector2 targetPosition = scrollRect.viewport.InverseTransformPoint(target.position);
        Vector2 newPosition = contentPanel.anchoredPosition + (viewportPosition - targetPosition);

        // Directly set the new position
        contentPanel.anchoredPosition = new Vector2(contentPanel.anchoredPosition.x, newPosition.y);
    }


    public void ExitStore()
    {
        EventSystem.current.SetSelectedGameObject(null);
        UIStateSwapper.I.ReturnToPlaying();
    }

    public void OnEnteredStore()
    {
        StartCoroutine(BozUtilities.DoAfterRealTimeDelay(1f,
            () => EventSystem.current.SetSelectedGameObject(firstEntryInStore)));
    }

    public void UpdateLabels()
    {
        nextWaveButtonText.text = "Start Wave " + DifficultyManager.I.currentWave;
        totalCurrencyText.text = BozUtilities.FormatLargeNumber(PlayerStats.I.currency1);
        totalCurrencyText.gameObject.SetActive(false);
        totalCurrencyText.gameObject.SetActive(true);
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
            categoryRow.gameObject.name = upgradeSO.upgradeType.ToString();

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
            UpgradeCardButtons.Add(upgradeData.gameObject);
            //TODO:: When you add an item to a row, set up it's left and right navigation to anything else on that same row
            //

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

        //For the first time you set this up
        SetupButtonNavigation(rowsParent.transform);
    }

    private void SetupButtonNavigation(Transform parentRows)
    {
        List<Transform> rows = new List<Transform>();

        // Collect all children of rowParent into a list of transforms
        for (int i = 0; i < parentRows.childCount; i++)
        {
            rows.Add(parentRows.GetChild(i));
        }

        // Go through each transform in the rows list
        for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            Transform row = rows[rowIndex];

            // Iterate through all children starting from index 1
            for (int childIndex = 1; childIndex < row.childCount; childIndex++)
            {
                Transform child = row.GetChild(childIndex);
                Button button = child.GetComponent<Button>();

                if (button != null)
                {
                    Navigation nav = new Navigation();
                    nav.mode = Navigation.Mode.Explicit;

                    // Set left navigation
                    if (childIndex > 1)
                    {
                        Button leftButton = row.GetChild(childIndex - 1).GetComponent<Button>();
                        if (leftButton != null)
                        {
                            nav.selectOnLeft = leftButton;
                        }
                    }

                    // Set right navigation
                    if (childIndex < row.childCount - 1)
                    {
                        Button rightButton = row.GetChild(childIndex + 1).GetComponent<Button>();
                        if (rightButton != null)
                        {
                            nav.selectOnRight = rightButton;
                        }
                    }

                    // Set up navigation
                    if (rowIndex > 0)
                    {
                        Transform previousRow = rows[rowIndex - 1];
                        Button upButton = childIndex < previousRow.childCount
                            ? previousRow.GetChild(childIndex).GetComponent<Button>()
                            : previousRow.GetChild(previousRow.childCount - 1).GetComponent<Button>();
                        if (upButton != null)
                        {
                            nav.selectOnUp = upButton;
                        }
                    }

                    if (rowIndex < rows.Count - 1)
                    {
                        Transform nextRow = rows[rowIndex + 1];
                        Button downButton = childIndex < nextRow.childCount
                            ? nextRow.GetChild(childIndex).GetComponent<Button>()
                            : nextRow.GetChild(nextRow.childCount - 1).GetComponent<Button>();
                        if (downButton != null)
                        {
                            nav.selectOnDown = downButton;
                        }
                    }
                    else
                    {
                        nav.selectOnDown = startWaveButton;
                    }

                    button.navigation = nav;
                }
            }
        }

        // Setup navigation for startWaveButton
        if (rows.Count > 0)
        {
            Transform lastRow = rows[rows.Count - 1];
            Button lastButton = lastRow.GetChild(lastRow.childCount - 1).GetComponent<Button>();
            if (lastButton != null)
            {
                Navigation nav = new Navigation();
                nav.mode = Navigation.Mode.Explicit;
                nav.selectOnUp = lastButton;
                nav.selectOnLeft = lastButton;
                startWaveButton.navigation = nav;
            }

            // Set the selected GameObject to the first item on the first row
            if (rows[0].childCount > 1)
            {
                firstEntryInStore = rows[0].GetChild(1).gameObject;
            }
        }
    }

    private GameObject firstEntryInStore;


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

    private void UpdateBuyModeText(int amount)
    {
        buyModeText.text = "Buy Mode x" + amount;
    }

    [Command]
    private void DebugAddMoney(float amount)
    {
        PlayerStats.I.currency1 += amount;
        UpdateLabels();
        OnDebugUpdateStoreUI?.Invoke();
    }

    [Command]
    private void DebugGetRich()
    {
        PlayerStats.I.currency1 += 999999999;
        UpdateLabels();
        OnDebugUpdateStoreUI?.Invoke();
        UIStateSwapper.I.GoToStore();
    }
}