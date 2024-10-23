using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsMenu : Singleton<SettingsMenu>
{
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup musicGroup;

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    [SerializeField] private GameObject firstObjectSelectedOnLoad;

    // Reference to the AudioMixer (assuming you have one)
    public AudioMixer audioMixer;

    // Keys to store volume settings in PlayerPrefs
    private const string SFX_PREF_KEY = "SFXVolume";
    private const string MUSIC_PREF_KEY = "MusicVolume";

    private bool madeFirstAdjustment = false;

    public void OnOpenedSettings()
    {
        EventSystem.current.SetSelectedGameObject(firstObjectSelectedOnLoad);
    }

    // Start is called before the first frame update
    private void Start()
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
        if (!madeFirstAdjustment)
        {
            madeFirstAdjustment = true;
            return;
        }

        SFXPlayer.I.Play(AudioEventsStorage.I.changedSFXVolume);
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


    public void Quit()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }
}