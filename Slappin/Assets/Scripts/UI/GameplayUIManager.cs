using System;
using QFSW.QC;
using TMPro;
using UnityEngine;

public class GameplayUIManager : Singleton<GameplayUIManager>
{
    [SerializeField] public TextMeshProUGUI currency1;
    [SerializeField] public TextMeshProUGUI countdownTimer;
    [SerializeField] public float maxWaveTimer = 60f;

    private float timeRemaining = 30f; // 60 seconds (1 minute)
    private bool timerRunning = true;
    private bool waveEndingAnnouncementMade = false;

    public static Action StartedNewWave;

    private void Start()
    {
        SFXPlayer.I.Play(AudioEventsStorage.I.gameStarted);
        StartNewWave();
    }

    public void StartNewWave()
    {
        //TODO:: Some sort of buffer screen first announcing the wave number
        timeRemaining = maxWaveTimer;
        timerRunning = true;
        InGameMessageAnnouncer.I.MakeAnouncement($"Wave {DifficultyManager.I.currentWave}",
            2f, 1f);
        waveEndingAnnouncementMade = false;
        StartedNewWave?.Invoke();
    }

    private void Update()
    {
        // Check if the timer is running
        if (timerRunning)
        {
            // Decrease the remaining time by the time passed since the last frame
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 6.5f && !waveEndingAnnouncementMade)
            {
                InGameMessageAnnouncer.I.MakeAnouncement($"5 seconds left!",
                    2f, 1f);
                waveEndingAnnouncementMade = true;
            }
            
            // If time has run out, stop the timer and trigger the function
            if (timeRemaining <= 0)
            {
                timeRemaining = 0; // Clamp the value to 0 so it doesn't go negative
                timerRunning = false; // Stop the timer
                OnTimerEnd(); // Trigger the end-of-timer function
            }

            // Update the timer display
            UpdateTimerDisplay(timeRemaining);
        }
    }

    [Command]
    private void DebugDropTimer()
    {
        timeRemaining = .5f;
    }

    // Updates the TextMeshPro text with the remaining time formatted as "mm:ss:ff"
    private void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60); // Get the remaining minutes
        int seconds = Mathf.FloorToInt(time % 60); // Get the remaining seconds
        int milliseconds = Mathf.FloorToInt((time * 100) % 100); // Get two digits of milliseconds

        // Display the time in "mm:ss:<small ms>" format with the milliseconds in a smaller size
        countdownTimer.text = string.Format("{0:00}:{1:00}:<size=70%>{2:00}</size>", minutes, seconds, milliseconds)
                              + "\tWave " + DifficultyManager.I.currentWave;
    }

    // Function to run when the timer hits zero
    private void OnTimerEnd()
    {
        Debug.Log("Timer has ended!");
        //TODO:: Some sort of buffer screen first announcing the wave number has ended
        DifficultyManager.I.SetupNextWave();
        UIStateSwapper.I.GoToStore();
    }

}