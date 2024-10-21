using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject contents;

    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup musicGroup;

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    // Reference to the AudioMixer (assuming you have one)
    public AudioMixer audioMixer;

    // Keys to store volume settings in PlayerPrefs
    private const string SFX_PREF_KEY = "SFXVolume";
    private const string MUSIC_PREF_KEY = "MusicVolume";

    // Start is called before the first frame update
    void Start()
    {
        // Load saved values from PlayerPrefs or set default to 1 (full volume)
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_PREF_KEY, 1f);
        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_PREF_KEY, 1f);

        // Apply the values to the AudioMixer
        SetSFXVolume(sfxSlider.value);
        SetMusicVolume(musicSlider.value);

        // Add listeners to sliders
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    // Method to set SFX volume and save to PlayerPrefs
    public void SetSFXVolume(float volume)
    {
        if (volume == 0f)
        {
            audioMixer.SetFloat("Music", -80f); // Convert linear slider value to decibel
        }
        else
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(sfxSlider.value) * 20); // Convert linear slider value to decibel
        }

        PlayerPrefs.SetFloat(SFX_PREF_KEY, sfxSlider.value);
    }

    // Method to set Music volume and save to PlayerPrefs
    public void SetMusicVolume(float volume)
    {
        if (volume == 0f)
        {
            audioMixer.SetFloat("Music", -80f); // Convert linear slider value to decibel
        }
        else
        {
            audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20); // Convert linear slider value to decibel
        }

        PlayerPrefs.SetFloat(MUSIC_PREF_KEY, volume);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause") || Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPrefs.Save();
            if (Time.timeScale == 1f)
            {
                Pause();
            }
            else
            {
                UnPause();
            }
        }
    }

    public void Pause()
    {
        MusicPlayer.I.Play(AudioEventsStorage.I.store);
        contents.SetActive(true);
        Debug.Log("UNPAUSING GAME");
        Time.timeScale = 0f;
        StateGame.PlayerInGameControlsEnabled = false;
    }

    public void UnPause()
    {
        MusicPlayer.I.Play(AudioEventsStorage.I.playing);
        contents.SetActive(false);
        Debug.Log("PAUSING GAME");
        Time.timeScale = 1f;
        StateGame.PlayerInGameControlsEnabled = true;
    }

    public void Quit()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }
}