using System.Collections;
using QFSW.QC;
using UnityEngine;

public class MusicPlayer : AudioPlayer
{
    public static MusicPlayer I { get; private set; }

    private AudioSource _nowPlayingAudioSource;
    private MusicScrob _previousMusic, _nowPlaying;
    private Coroutine playMusicCoroutine;

    [SerializeField] private AnimationCurve fadeOutCurve;
    [SerializeField] private AnimationCurve fadeInCurve;


    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
        }

        I = this;
    }

    public void Play(MusicScrob musicToPlay, float startTime = 0f)
    {
        _nowPlayingAudioSource ??= GetFreeAudioSource();
        if (musicToPlay == _nowPlaying) return;
        _previousMusic ??= musicToPlay; //Only set this up on the first time

        if (playMusicCoroutine is not null) StopCoroutine(playMusicCoroutine);
        playMusicCoroutine = StartCoroutine(CrossFade(_nowPlayingAudioSource, musicToPlay, startTime));
    }


    private IEnumerator CrossFade(AudioSource oldSource, MusicScrob musicScrob, float startTime = 0f)
    {
        if (oldSource is null || musicScrob is null)
            yield break;

        float fadeOutDuration = musicScrob.fadeOutTime;
        float fadeInDuration = musicScrob.fadeInTime; //TODO:: Implement this
        float fadeTime = fadeOutDuration;
        float endTime = Time.time + fadeTime;
        float startVolume = oldSource.volume;

        _nowPlayingAudioSource = GetFreeAudioSource();
        _nowPlayingAudioSource.playOnAwake = false;
        _nowPlayingAudioSource.clip = musicScrob.clip;
        _nowPlayingAudioSource.outputAudioMixerGroup = musicScrob.mixerGroup;
        _nowPlayingAudioSource.volume = 0f;
        _nowPlayingAudioSource.time = startTime;
        _nowPlayingAudioSource.loop = musicScrob.looping;
        _nowPlayingAudioSource.Play();

        if (_nowPlaying is not null)
        {
            _nowPlayingAudioSource.time = oldSource.time;

            float currentTime = 0f;
            float crossfadeDuration = 3f;

            while (currentTime < crossfadeDuration)
            {
                //THIS ISNT EXITING
                currentTime += Time.unscaledDeltaTime;
                float t = currentTime / crossfadeDuration;

                // Lerp volumes between 1 (source1) and 0 (source2) based on time
                oldSource.volume = Mathf.Lerp(_nowPlaying.volume, 0f, t);
                _nowPlayingAudioSource.volume = Mathf.Lerp(0f, musicScrob.volume, t);

                yield return null;
            }

            oldSource.Stop();
            oldSource.clip = null;
            oldSource.volume = 0;
        }

        _previousMusic = _nowPlaying;

        _nowPlayingAudioSource.volume = musicScrob.volume;
        _nowPlaying = musicScrob;
        Debug.Log("Now Playing: " + musicScrob.name);
    }


    public void Stop()
    {
        _nowPlayingAudioSource.volume = 0;
    }
}