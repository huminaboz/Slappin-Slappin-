using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public abstract class AudioPlayer : MonoBehaviour
{
    // size of the pool of audio sources to keep consistently once created
    [SerializeField] private int maxSimultaneousSFXs = 4;

    // protected List<AudioClip> playingClips = new List<AudioClip>();
    [SerializeField] protected AudioMixerGroup audioGroup;

    private List<AudioSource> sourcesNotInUse = new List<AudioSource>();
    private List<AudioSource> sourcesInUse = new List<AudioSource>();


    protected AudioSource DoPlay(SFXScrob sfxScrob)
    {
        if (sfxScrob.clip is null)
        {
            Debug.LogError("There was no audio clip on soundScrob: " + sfxScrob.name);
            return null;
        }

        AudioSource source = GetFreeAudioSource();
        if (source is null) return null;

        source.outputAudioMixerGroup = sfxScrob.mixerGroup;
        if (source.outputAudioMixerGroup == null) source.outputAudioMixerGroup = audioGroup;

        if (sfxScrob.varyPitch)
        {
            source.pitch = Random.Range(.9f, 1f);
        }

        source.clip = sfxScrob.clip;
        source.volume = sfxScrob.volume;
        source.loop = sfxScrob.looping;

        source.Play();
        return source;
    }

    private bool ExceedingCapacity()
    {
        return sourcesInUse.Count > maxSimultaneousSFXs;
    }

    protected AudioSource GetFreeAudioSource()
    {
        //First, clean up any unused AudioSources
        for (int i = sourcesInUse.Count - 1; i > -1; i--)
        {
            if (sourcesInUse[i].isPlaying) continue;
            sourcesNotInUse.Add(sourcesInUse[i]);
            sourcesInUse.RemoveAt(i);
        }

        //If there are too many
        if (ExceedingCapacity()) return null;

        //There are too few
        if (sourcesNotInUse.Count < 1)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            sourcesNotInUse.Add(newSource);
        }

        //Finally, get a free source from the bottom
        int index = sourcesNotInUse.Count - 1;
        AudioSource nextSource = sourcesNotInUse[index];
        sourcesInUse.Add(nextSource);
        sourcesNotInUse.RemoveAt(index);

        return nextSource;
    }


    protected void DoStop(AudioScrob audioScrob)
    {
        foreach (AudioSource src in sourcesInUse)
        {
            if (src.clip == audioScrob.clip)
                src.Stop();
        }
    }
}