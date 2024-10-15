using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public abstract class AudioPlayer : MonoBehaviour
{
    // size of the pool of audio sources to keep consistently once created
    [SerializeField] private int maxSimultaneousSFXs = 5;

    //Needed in case I need to stop the clips from playing
    private List<AudioClip> _playingAudioClips = new List<AudioClip>();
    
    [SerializeField] protected AudioMixerGroup audioGroup;

    private List<AudioSource> _sourcesNotInUse = new List<AudioSource>();
    private List<AudioSource> _sourcesInUse = new List<AudioSource>();

    protected AudioSource DoPlay(SFXScrob sfxScrob)
    {
        //First, clean up any unused AudioSources
        for (int i = _sourcesInUse.Count - 1; i > -1; i--)
        {
            if (_sourcesInUse[i].isPlaying) continue;
            if (ExceedingCapacity())
            {
                Destroy(_sourcesInUse[i]);
            }
            else _sourcesNotInUse.Add(_sourcesInUse[i]);
            _playingAudioClips.Remove(_sourcesInUse[i].clip);
            _sourcesInUse.RemoveAt(i);
        }
        
        if (sfxScrob.clip is null)
        {
            Debug.LogError("There was no audio clip on soundScrob: " + sfxScrob.name);
            return null;
        }

        AudioSource source = GetFreeAudioSource();

        source.outputAudioMixerGroup = sfxScrob.mixerGroup;
        if (source.outputAudioMixerGroup == null) source.outputAudioMixerGroup = audioGroup;

        source.pitch = 1f; //reset the pitch
        if (sfxScrob.varyPitch)
        {
            source.pitch = 1f + Random.Range(-.06f, .06f);
        }

        source.clip = sfxScrob.clip;
        source.volume = sfxScrob.volume;
        source.loop = sfxScrob.looping;

        source.Play();
        _playingAudioClips.Add(sfxScrob.clip);
        return source;
    }

    private bool ExceedingCapacity()
    {
        return _sourcesInUse.Count > maxSimultaneousSFXs;
    }

    // private AudioSource GetSourceWithSameClip(AudioClip clip)
    // {
    //     foreach (AudioSource audioSource in sourcesInUse)
    //     {
    //         if (audioSource.clip == clip)
    //         {
    //             return audioSource;
    //         }
    //     }
    //
    //     Debug.Log("Couldn't find an audiosource with the same clip, despite knowing the list of audioscources contains it");
    //     return null;
    // }

    protected AudioSource GetFreeAudioSource()
    {
        //If there are too few - spawn a new one
        if (_sourcesNotInUse.Count < 1)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            _sourcesNotInUse.Add(newSource);
        }

        //Finally, get a free source from the bottom
        int index = _sourcesNotInUse.Count - 1;
        AudioSource nextSource = _sourcesNotInUse[index];
        _sourcesInUse.Add(nextSource);
        _sourcesNotInUse.RemoveAt(index);

        return nextSource;
    }



}