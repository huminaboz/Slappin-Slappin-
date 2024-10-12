using System;
using System.Collections;
using UnityEngine;

public class SFXPlayer : AudioPlayer
{
    public static SFXPlayer I { get; private set; }

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
        }

        I = this;
    }

    public AudioSource Play(SFXScrob sfxScrob)
    {
        if (sfxScrob is not null)
        {
            if (sfxScrob.startDelaySeconds > 0f)
            {
                StartCoroutine(PlayAfterDelay(sfxScrob.startDelaySeconds, 
                    () => DoPlay(sfxScrob)));
            }
            else return DoPlay(sfxScrob);
        }
        else
        {
            Debug.LogWarning("Tried to play a null Soundscrob: Add one or set a default");
        }
        return null;
    }
    
   
    private IEnumerator PlayAfterDelay(float secondsDelay, Func<AudioSource> audioSource)
    {
        yield return new WaitForSeconds(secondsDelay);
        audioSource?.Invoke();
    }
    
   
    // public bool IsPlaying(SFXScrob sfxScrob)
    // {
    //     return playingClips.Contains(sfxScrob.clip);
    // }
    
    // public static void Stop(AudioClip _clip)
    // {
    //     if (_clip == null) return;
    //     
    //     DoStop(_clip);
    // }
}