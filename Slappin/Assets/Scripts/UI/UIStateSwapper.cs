using QFSW.QC;
using UnityEngine;

public class UIStateSwapper : Singleton<UIStateSwapper>
{
    [SerializeField] private GameObject storeUI;
    [SerializeField] private GameObject playingHUD;
    
    //TODO:: Create a pause screen
    [SerializeField] private GameObject pauseScreen;
    
    //TODO:: Create a lose screen
    [SerializeField] private GameObject loseScreen;

    [SerializeField] private Player player;
    
    public enum UIState
    {
        playing,
        store,
        paused,
        youLose
    }
    
    private UIState currentUIState = UIState.playing;

    public void SetState(UIState state)
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

    private void SetupUIState()
    {
        storeUI.SetActive(currentUIState == UIState.store);
        if(currentUIState == UIState.store) StoreUIManager.I.UpdateLabels();
        playingHUD.SetActive(currentUIState == UIState.playing);
        if (currentUIState == UIState.playing)
        {
            PlayerStats.I.UpdateHUD();
        }
        
        //TODO:: Create a pause screen
        // pauseScreen.SetActive(currentUIState == UIState.paused);
        //TODO:: Create a lose screen
        // loseScreen.SetActive(currentUIState == UIState.youLose);
    }
}