using System;
using QFSW.QC;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIStateSwapper : Singleton<UIStateSwapper>
{
    [SerializeField] private GameObject storeUI;
    [SerializeField] private GameObject playingHUD;

    [SerializeField] private GameObject pauseScreen;

    [SerializeField] private Player player;

    [SerializeField] private TextMeshProUGUI modalText;
    [SerializeField] private GameObject modalActivator;

    private bool restartingEnabled = false;
    
    public enum UIState
    {
        playing,
        store,
        paused, //There's no need for a paused state yet because pause can return to any state with a button press
        youLose
    }

    private UIState currentUIState = UIState.playing;

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
        if (currentUIState == UIState.store) StoreUIManager.I.UpdateLabels();
        playingHUD.SetActive(currentUIState == UIState.playing);
        if (currentUIState == UIState.playing)
        {
            PlayerStats.I.UpdateHUD();
        }

        modalActivator.SetActive(currentUIState == UIState.youLose);
        if (currentUIState == UIState.youLose)
        {
            SetModalMessage("Hands Down.");
            StartCoroutine(BozUtilities.DoAfterRealTimeDelay(1f, () =>
            {
                restartingEnabled = true;
            }));
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause") || Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPrefs.Save();
            if (!pauseScreen.activeSelf)
            {
                Pause();
            }
            else
            {
                UnPause();
            }
        }

        if (currentUIState == UIState.youLose && restartingEnabled)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                //TODO:: Reload the scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    public void Pause()
    {
        MusicPlayer.I.Play(AudioEventsStorage.I.store);
        pauseScreen.SetActive(true);
        Debug.Log("PAUSING GAME and showing settings menu");
        Time.timeScale = 0f;
        StateGame.PlayerInGameControlsEnabled = false;
    }

    public void UnPause()
    {
        pauseScreen.SetActive(false);
        Debug.Log("Removing settings screen and returning to whatever screen you were on");
        if (currentUIState == UIState.playing)
        {
            MusicPlayer.I.Play(AudioEventsStorage.I.playing);
            Time.timeScale = 1f;
            StateGame.PlayerInGameControlsEnabled = true;
        }
    }
}