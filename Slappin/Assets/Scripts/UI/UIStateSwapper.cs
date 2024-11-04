using System;
using QFSW.QC;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIStateSwapper : Singleton<UIStateSwapper>
{
    [SerializeField] private GameObject storeUI;
    [SerializeField] private GameObject playingHUD;

    [SerializeField] private GameObject pauseScreen;

    [SerializeField] private Player player;

    [SerializeField] private TextMeshProUGUI modalText;
    [SerializeField] private GameObject modalActivator;
    [SerializeField] private GameObject controlsScreen;

    [SerializeField] private GameObject exitButtonIcon;

    private bool restartingEnabled = false;

    public static Action OnEnterStore;
    
    private InputSystem_Actions _inputSystem;
    
    public enum UIState
    {
        playing,
        store,
        paused, //There's no need for a paused state yet because pause can return to any state with a button press
        youLose
    }

    public UIState currentUIState = UIState.playing;

    private void Awake()
    {
        _inputSystem = new InputSystem_Actions();
        _inputSystem.Player.Enable();
    }

    private void OnEnable()
    {
        Health.OnDeath += SetLoseState;
    }

    private void OnDisable()
    {
        Health.OnDeath -= SetLoseState;
    }

    private void Start()
    {
        SetState(UIState.playing);

        //Skip the controls screen if debugging 
        if (StateGame.debugModeOn) return;
        controlsScreen.SetActive(!controlsScreen.activeSelf);
        TurnOffGameplay();
    }

    private void SetState(UIState state)
    {
        currentUIState = state;

        StateGame.PlayerInGameControlsEnabled = state == UIState.playing;

        //TODO:: Make sure none of the UI depends on Time.timescale
        Time.timeScale = currentUIState == UIState.playing ? 1f : 0f;

        if (currentUIState != UIState.playing)
        {
            player.SetState(new StateEmpty(player));
        }
        else
        {
            player.SetState(new StateDefault(player));
        }

        SetupUIState();
    }

    [Command]
    public void GoToStore()
    {
        SetState(UIState.store);
        MusicPlayer.I.Play(AudioEventsStorage.I.store);
        SFXPlayer.I.Play(AudioEventsStorage.I.WaveEnded);
    }

    public void ReturnToPlaying()
    {
        MusicPlayer.I.Play(AudioEventsStorage.I.playing);
        SFXPlayer.I.Play(AudioEventsStorage.I.WaveStart);

        SetState(UIState.playing);
        GameplayUIManager.I.StartNewWave();
    }

    private void SetLoseState()
    {
        Health.OnDeath -= SetLoseState;
        SetState(UIState.youLose);
        MusicPlayer.I.Play(AudioEventsStorage.I.store);
    }

    private void SetModalMessage(string message)
    {
        modalText.text = message;
    }

    private void SetupUIState()
    {
        storeUI.SetActive(currentUIState == UIState.store);
        playingHUD.SetActive(currentUIState == UIState.playing);
        modalActivator.SetActive(currentUIState == UIState.youLose);

        switch (currentUIState)
        {
            case UIState.playing:
                PlayerStats.I.UpdateHUD();
                break;
            case UIState.store:
                StoreUIManager.I.UpdateLabels();
                StoreUIManager.I.OnEnteredStore();
                canExitStore = false;
                exitButtonIcon.SetActive(false);
                StartCoroutine(BozUtilities.DoAfterRealTimeDelay(2f, SetStoreExitPossibility));
                OnEnterStore?.Invoke();
                break;
            case UIState.youLose:
                SetModalMessage($"Hands Down" +
                                $"\non Wave <color=#746CA5>{DifficultyManager.I.currentWave}</color>." +
                                $"\n\n<color=#FF6C4B><size=45%>Post it in the comments!</size></color>");
                StartCoroutine(BozUtilities.DoAfterRealTimeDelay(2f, () =>
                {
                    //TODO:: Show the button you can press
                    restartingEnabled = true;
                }));
                break;
        }
    }

    private bool canExitStore = false;

    private void SetStoreExitPossibility()
    {
        canExitStore = true;
        //Show the exit icon
        exitButtonIcon.SetActive(true);
    }

    private void Update()
    {
        if (_inputSystem.Player.Absorb.WasPerformedThisFrame())
        {
            if (currentUIState == UIState.store && !pauseScreen.activeSelf
                                                && !controlsScreen.activeSelf && canExitStore)
            {
                StoreUIManager.I.ExitStore();
                canExitStore = false;
            }
        }

        if (_inputSystem.Player.Settings.WasPerformedThisFrame())
        {
            PlayerPrefs.Save();
            if (!pauseScreen.activeSelf)
            {
                if (controlsScreen.activeSelf) controlsScreen.SetActive(false);
                Pause();
            }
            else
            {
                UnPause();
            }
        }

        if (_inputSystem.Player.Instructions.WasPerformedThisFrame())
        {
            if (!pauseScreen.activeSelf)
            {
                controlsScreen.SetActive(!controlsScreen.activeSelf);
                if (controlsScreen.activeSelf)
                {
                    TurnOffGameplay();
                }
                else
                {
                    if (currentUIState != UIState.store)
                    {
                        TurnOnGameplay();
                    }
                }
            }
        }

        if (currentUIState == UIState.youLose && restartingEnabled)
        {
            if (_inputSystem.Player.Slap.WasPerformedThisFrame())
            {
                //Reload the scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                //TODO:: Clean up static variables
            }
        }
    }

    public void Pause()
    {
        MusicPlayer.I.Play(AudioEventsStorage.I.store);
        pauseScreen.SetActive(true);
        SettingsMenu.I.OnOpenedSettings();
        Debug.Log("PAUSING GAME and showing settings menu");
        TurnOffGameplay();
    }

    private void TurnOffGameplay()
    {
        Time.timeScale = 0f;
        StateGame.PlayerInGameControlsEnabled = false;
    }

    private void TurnOnGameplay()
    {
        Time.timeScale = 1f;
        StateGame.PlayerInGameControlsEnabled = true;
    }

    public void UnPause()
    {
        pauseScreen.SetActive(false);
        Debug.Log("Removing settings screen and returning to whatever screen you were on");
        if (currentUIState == UIState.playing)
        {
            MusicPlayer.I.Play(AudioEventsStorage.I.playing);
            TurnOnGameplay();
        }

        if (currentUIState == UIState.store)
        {
            StoreUIManager.I.OnEnteredStore();
        }
    }
}