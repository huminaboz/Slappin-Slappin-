using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public abstract class AudioPlayer : MonoBehaviour
{
    private List<AudioSource> audioSources = new List<AudioSource>();

    // size of the pool of audio sources to keep consistently once created
    [SerializeField] private int sourcePoolSize = 4;

    protected List<AudioClip> playingClips = new List<AudioClip>();
    [SerializeField] protected AudioMixerGroup audioGroup;


    private void Update()
    {
        Debug.Log("test");
        playingClips.Clear();

        foreach (AudioSource thisSource in audioSources.ToArray())
        {
            if (!thisSource.isPlaying)
            {
                if (audioSources.Count <= sourcePoolSize) continue;
                audioSources.Remove(thisSource);
                Destroy(thisSource); //TODO:: Make a pool
            }
            else playingClips.Add(thisSource.clip);
        }
    }

    protected AudioSource DoPlay(SFXScrob sfxScrob)
    {
        if (sfxScrob.clip is null)
        {
            Debug.LogError("There was no audio clip on soundScrob: " + sfxScrob.name);
            return null;
        }

        AudioSource source = GetFreeAudioSource();

        source.outputAudioMixerGroup = sfxScrob.mixerGroup;
        if (source.outputAudioMixerGroup == null) source.outputAudioMixerGroup = audioGroup;

        if (sfxScrob.varyPitch)
        {
            //TODO:: Handle varying pitch
        }

        source.clip = sfxScrob.clip;
        source.volume = sfxScrob.volume;
        source.loop = sfxScrob.looping;

        source.Play();
        return source;
    }

    protected void DoStop(AudioClip _clip)
    {
        foreach (AudioSource src in audioSources)
        {
            if (src.clip == _clip)
                src.Stop();
        }
    }

    protected AudioSource GetFreeAudioSource()
    {
        foreach (AudioSource source in audioSources)
        {
            if (!source.isPlaying) return source;
        }

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        audioSources.Add(newSource);

        return newSource;
    }
}