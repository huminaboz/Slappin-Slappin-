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
    private void GoToStore()
    {
        SetState(UIState.store);
    }
    
    public void ReturnToPlaying()
    {
        //TODO:: Enemy stuff states new waves and etc etc
        SetState(UIState.playing);
    }

    private void SetupUIState()
    {
        storeUI.SetActive(currentUIState == UIState.store);
        if(currentUIState == UIState.store) StoreUIManager.I.UpdateLabels();
        playingHUD.SetActive(currentUIState == UIState.playing);
        
        //TODO:: Create a pause screen
        // pauseScreen.SetActive(currentUIState == UIState.paused);
        //TODO:: Create a lose screen
        // loseScreen.SetActive(currentUIState == UIState.youLose);
    }
}